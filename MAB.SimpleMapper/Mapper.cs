using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Concurrent;

namespace MAB.SimpleMapper
{
    public static class Mapper
    {
        #region Member Variables

        // If the type is in this namespace, it's probably an Entity Framework dynamic proxy, so 
        // won't match any custom mappings keyed on the base type. This lets us test for that.
        private const string _EF_DYNAMIC_PROXY_PREFIX = "System.Data.Entity.DynamicProxies";

        // Tuple-keyed dictionary to allow us to look up custom mappings based on the combination 
        // of source and destination type. This also allows us to set up distinct mappings for 
        // mapping to and from the same type
        private static ConcurrentDictionary<Tuple<Type, Type>, object> _maps = new ConcurrentDictionary<Tuple<Type, Type>, object>();

        // Tuple-keyed dictionary to allow us to look up cached constructor 
        // expressions based on the combination of source and destination type
        private static ConcurrentDictionary<Tuple<Type, Type>, Tuple<object, ConstructorInfo>> _activators = new ConcurrentDictionary<Tuple<Type, Type>, Tuple<object, ConstructorInfo>>();

        #endregion Member Variables

        #region Support Methods

        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        // Uppercase and remove underscores from property names to allow (sort of) fuzzy matching
        private static string GetNormalisedPropertyName(string propertyName)
        {
            return Regex.Replace(propertyName.ToUpperInvariant(), "_", "");
        }

        // As the number of parameters is unknown, we can't return Func<TSource, TDestination>
        // from CreateObject<T> (because there are only a finite number of overloads), so we 
        // set up a delegate which can be called to create objects
        private delegate T CreateT<T>(params object[] args);

        // Returns a delegate expression which will create an object 
        // of the specified type using the specified constructor
        private static CreateT<T> CreateObject<T>(ConstructorInfo constructor)
        {
            var type = constructor.DeclaringType;
            var constructorParams = constructor.GetParameters();

            // Here's our object[] args parameter, as an expression
            var args = Expression.Parameter(typeof(object[]), "args");

            // For each constructor parameter, create an expression which 
            // converts the object at the same index in the args array to 
            // the correct type for the parameter
            var argExpressions = constructorParams.Select((p, i) => Expression.Convert(
                Expression.ArrayIndex(args, Expression.Constant(i)),
                constructorParams[i].ParameterType
            )).ToArray();

            // Create the 'new' expression using the constructor and the 
            // array of convert expressions for each parameter
            var newExp = Expression.New(constructor, argExpressions);

            // Create a lambda expression which takes an object[] and passes
            // it to our generated 'new' expression
            var lambda = Expression.Lambda(typeof(CreateT<T>), newExp, args);

            // Construct and return a CreateT<T>(params object[] args) body
            return (CreateT<T>)lambda.Compile();
        }

        #endregion Support Methods

        #region Custom Mapping Lookups
        
        /// <summary>
        /// Add a custom mapping between a particular source and destination type
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="map">Delegate which performs the custom mapping</param>
        public static void AddMapping<TSource, TDestination>(Action<TSource, TDestination> map)
            where TSource : class
            where TDestination : class
        {
            // Add the mapping keyed on a combination of the source and destination types
            _maps.TryAdd(Tuple.Create(typeof(TSource), typeof(TDestination)), map);
        }

        /// <summary>
        /// Clear all custom mappings
        /// </summary>
        public static void ClearMappings()
        {
            _maps.Clear();
        }

        #endregion Custom Mapping Lookups

        #region Property Mapping Methods

        /// <summary>
        /// Map a source object of type TSource to a new object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>New object of type TDestination</returns>
        private static TDestination MapNew<TSource, TDestination>(TSource source, params object[] constructorParameters)
            where TSource : class
            where TDestination : class
        {
            if (source == null)
                return default(TDestination);

            // Create a new instance of the destination type
            var destination = (constructorParameters == null || constructorParameters.Length == 0) 
                            ? Activator.CreateInstance<TDestination>()
                            : (TDestination)Activator.CreateInstance(typeof(TDestination), constructorParameters);

            // Map the source object to the new destination instance
            MapExisting<TSource, TDestination>(source, destination);

            return destination;
        }

        /// <summary>
        /// Map a source object of type TSource to an existing object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        private static void MapExisting<TSource, TDestination>(TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);

            // If either type is an Entity Framework dynamic proxy, we need to get the base type
            // so that the key matches any custom mappings set up for it
            if (sourceType.Namespace != null && sourceType.Namespace.StartsWith(_EF_DYNAMIC_PROXY_PREFIX))
                sourceType = sourceType.BaseType;

            if (destinationType.Namespace != null && destinationType.Namespace.StartsWith(_EF_DYNAMIC_PROXY_PREFIX))
                destinationType = destinationType.BaseType;

            // Create a composite key from the source and destination types
            var key = Tuple.Create(sourceType, destinationType);

            // Look for a custom mapping for this source/destination type combination
            Action<TSource, TDestination> map = (_maps.ContainsKey(key)) ? _maps[key] as Action<TSource, TDestination> : null;

            if (map == null)
            {
                // There's no specific mapping set up, so we'll create one by convention
                var sourceProperties = source.GetType().GetProperties();
                var destinationProperties = destination.GetType().GetProperties();
                
                // Test to determine whether a property of the destination type can/should be assigned to
                Func<PropertyInfo, bool> isMappableProperty = dst => dst.CanWrite 
                                                                  && sourceProperties.Any(src => GetNormalisedPropertyName(dst.Name).Equals(GetNormalisedPropertyName(src.Name))
                                                                                              && ((dst.PropertyType == src.PropertyType) || (dst.PropertyType.IsEnum && src.PropertyType.IsEnum)));

                // Create expression parameter references for the source and destination instance parameters
                var sourceInstance = Expression.Parameter(typeof(TSource), "source");
                var destinationInstance = Expression.Parameter(typeof(TDestination), "destination");

                // Get a list of source -> destination property assignment expressions
                // Note that if the property is an enum, a converted value is assigned rather than a direct property value
                var propertyAssignments = destinationProperties.Where(isMappableProperty).Select(p => Expression.Assign(
                    Expression.Property(destinationInstance, p.Name),
                    p.PropertyType.IsEnum ? (Expression)Expression.Convert(Expression.Property(sourceInstance, p.Name), p.PropertyType)
                                          : Expression.Property(sourceInstance, p.Name)
                )); 
                
                // Create an expression containing all our property assignments and passing in the
                // source and destination object instances as arguments
                var expression = Expression.Lambda<Action<TSource, TDestination>>(
                    Expression.Block(propertyAssignments), 
                    sourceInstance, 
                    destinationInstance
                );

                // Compile the assignment expression and cache the resulting delegate in the mappings dictionary
                map = expression.Compile();
                AddMapping<TSource, TDestination>(map);
            }

            // Map using the stored delegate for this source/destination type combination
            map(source, destination);
        }

        /// <summary>
        /// Map a list of objects of type TSource to a new list of objects of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source list</param>
        /// <returns>New list of objects of type TDestination</returns>
        private static List<TDestination> MapList<TSource, TDestination>(List<TSource> source, params object[] constructorParameters)
            where TSource : class
            where TDestination : class
        {
            var destination = new List<TDestination>();

            foreach (var item in source)
                destination.Add(MapNew<TSource, TDestination>(item, constructorParameters));

            return destination;
        }

        /// <summary>
        /// Map a source object to a destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        public static void MapTo(this object source, object destination)
        {
            if (source != null)
            {
                // Use reflection to get the Map<TSource, TDestination>(TSource source, TDestination destination) method
                MethodInfo method = typeof(Mapper).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                                                  .Where(x => x.Name == "MapExisting")
                                                  .First();

                // Get the types of the source and destination objects and pass them into the method as type parameters
                MethodInfo generic = method.MakeGenericMethod(new Type[] { source.GetType(), destination.GetType() });

                // Invoke the Map method, passing in the source and destination objects as method parameters
                generic.Invoke(null, new object[] { source, destination });
            }
        }

        /// <summary>
        /// Map a source object to a new destination object of type TDestination
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapTo<TDestination>(this object source, params object[] constructorParameters)
            where TDestination : class
        {
            if (source == null)
                return null;

            // Use reflection to get the Map<TSource, TDestination>(TSource source) method
            MethodInfo method = typeof(Mapper).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                                              .Where(x => x.Name == "MapNew")
                                              .First();

            // Get the type of the source objects and pass it into the method as a type parameter,
            // along with the type of the new destination object
            MethodInfo generic = method.MakeGenericMethod(new Type[] { source.GetType(), typeof(TDestination) });

            // Invoke the Map method, passing in the source object as a method parameter, and 
            // return the resulting object of type TDestination
            return (TDestination)generic.Invoke(null, new object[] { source, constructorParameters });
        }

        /// <summary>
        /// Map a list of source objects to a new list of objects of type TDestination
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source list</param>
        /// <returns>New list of objects of type TDestination</returns>
        public static List<TDestination> MapToList<TDestination>(this object source, params object[] constructorParameters)
            where TDestination : class
        {
            if (source == null)
                return null;

            // Use reflection to get the MapList<TSource, TDestination>(TSource source) method
            MethodInfo method = typeof(Mapper).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                                              .Where(x => x.Name == "MapList")
                                              .First();

            // Get the type argument of the source list and pass it into the method as a type parameter,
            // along with the type of the new destination list
            MethodInfo generic = method.MakeGenericMethod(new Type[] { source.GetType().GetGenericArguments()[0], typeof(TDestination) });

            // Invoke the MapList method, passing in the source list as a method parameter, and 
            // return the resulting list of objects of type TDestination
            return (List<TDestination>)generic.Invoke(null, new object[] { source, constructorParameters });
        }

        /// <summary>
        /// Map a list of source objects to a new list of objects of type TDestination
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source list</param>
        /// <returns>New list of objects of type TDestination</returns>
        public static IEnumerable<TDestination> MapToEnumerable<TDestination>(this object source, params object[] constructorParameters)
            where TDestination : class
        {
            if (source == null)
                return null;

            // Use reflection to get the MapList<TSource, TDestination>(TSource source) method
            MethodInfo method = typeof(Mapper).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                                              .Where(x => x.Name == "MapList")
                                              .First();

            // Get the type argument of the source list and pass it into the method as a type parameter,
            // along with the type of the new destination list
            MethodInfo generic = method.MakeGenericMethod(new Type[] { source.GetType().GetGenericArguments()[0], typeof(TDestination) });

            // Invoke the MapList method, passing in the source list as a method parameter, and 
            // return the resulting list of objects of type TDestination
            return (IEnumerable<TDestination>)generic.Invoke(null, new object[] { source, constructorParameters });
        }

        #endregion Property Mapping Methods

        #region Constructor Mapping Methods

        /// <summary>
        /// Create a new object of type <typeparamref name="TDestination"/> by passing the property values of 
        /// <paramref name="source"/> as constructor parameters 
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapToConstructor<TDestination>(this object source) 
            where TDestination: class
        {
            if(source == null)
                return default(TDestination);

            return MapToConstructorCustomDelegate<TDestination>(source);
        }

        // This implementation is approximately 2x slower than MapToConstructorCreateInstance, but will
        // attempt to find a constructor signature which matches as many of the properties of the source 
        // object as possible by name and type (order is not considered). This means it can create instances 
        // from source objects with *any* number of properties, as long as the object being created allows 
        // default values to be passed in for any constructor parameters which can't be matched.
        // In common use cases perf isn't really an issue anyway (around 100ms slower creating 10,000 objects)
        internal static TDestination MapToConstructorCustomDelegate<TDestination>(this object source) 
            where TDestination: class
        {
            var sourceType = source.GetType();
            var destinationType = typeof(TDestination);

            var sourceProperties = sourceType.GetProperties();

            var key = Tuple.Create(sourceType, destinationType);

            // See if there's an entry in the activator cache for this source/destination combination
            var activator = (_activators.ContainsKey(key)) ? _activators[key] : null;

            if(activator == null)
            { 
                // Get all the constructors for the destination type
                var constructors = destinationType.GetConstructors();
        
                var constructorSignatures = new Dictionary<ConstructorInfo, IEnumerable<string>>();

                // For each constructor on this type, add a dictionary entry where the constructor itself 
                // is the key, and the value is a list of the parameters for the constructor, converted 
                // to strings in the form PROPNAME~TYPENAME
                foreach (var constructor in constructors)
                {
                    var constructorSignature = constructor.GetParameters().Select(p => string.Format("{0}~{1}", GetNormalisedPropertyName(p.Name), p.ParameterType));
                    constructorSignatures.Add(constructor, constructorSignature);
                }
        
                // Now we do the same with all the properties of the source type
                var sourcePropertySignature = sourceProperties.Select(p => string.Format("{0}~{1}", GetNormalisedPropertyName(p.Name), p.PropertyType));

                // This dictionary will hold a number telling us how different each constructor signature
                var rankedConstructorSignatures = new List<Tuple<ConstructorInfo, int>>();

                // Loop over each constructor signature and find the one which matches the most source type 
                // properties, by intersecting the array of property signatures for each constructor with the 
                // property signature of the current source object
                foreach (var k in constructorSignatures.Keys)
                    rankedConstructorSignatures.Add(new Tuple<ConstructorInfo, int>(k, sourcePropertySignature.Intersect(constructorSignatures[k]).Count()));
        
                // Pick the constructor with the largest intersection with the source type's properties
                var bestConstructor = rankedConstructorSignatures.OrderByDescending(x => x.Item2).First().Item1;
        
                // Create a delegate which will construct on object of the new type using the best matching constructor
                var createDelegate = CreateObject<TDestination>(bestConstructor);

                // Cache the delegate along with the best constructor, so we can use it again if the consumer  
                // needs to construct another new instance of TDestination from the same source type
                activator = new Tuple<object, ConstructorInfo>(createDelegate, bestConstructor);
                _activators.TryAdd(Tuple.Create(sourceType, destinationType), activator);
            }

            Func<PropertyInfo, ParameterInfo, bool> find = (sp, p) => GetNormalisedPropertyName(sp.Name) == GetNormalisedPropertyName(p.Name) && sp.PropertyType == p.ParameterType;
            Func<ParameterInfo, object> getValue = (p) => {
                var prop = sourceProperties.FirstOrDefault(sp => find(sp, p));
                return prop == null ? GetDefault(p.ParameterType) : prop.GetValue(source, null);
            };

            var args = activator.Item2.GetParameters().Select(p => getValue(p)).ToArray();
            
            return ((CreateT<TDestination>)activator.Item1)(args);
        }

        // This implementation is approx 2x faster than MapToConstructorCustomDelegate, but 
        // will only handle mapping from types where the properties match the constructor parameters
        // *exactly* (i.e. in number, order, name and type) 
        internal static TDestination MapToConstructorCreateInstance<TDestination>(this object source) 
            where TDestination : class
        {
            var sourceProperties = source.GetType().GetProperties().Select(p => p.GetValue(source, null)).ToArray();
        
            return (TDestination)Activator.CreateInstance(typeof(TDestination), sourceProperties);
        }

        #endregion Constructor Mapping Methods
    }
}
