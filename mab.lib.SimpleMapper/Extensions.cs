using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace mab.lib.SimpleMapper
{
    public static class Extensions
    {
        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of a new destination object
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapTo<TDestination>(this object source)
        {
            return MapTo<TDestination>(source, null, new List<string>(), null, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of a new destination object
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapTo<TDestination>(this object source, object[] constructorParameters)
        {
            return MapTo<TDestination>(source, constructorParameters, new List<string>(), null, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of a new destination object
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapTo<TDestination>(this object source, object[] constructorParameters, List<string> excludes)
        {
            return MapTo<TDestination>(source, constructorParameters, excludes, null, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of a new destination object
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapTo<TDestination>(this object source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction)
        {
            return MapTo<TDestination>(source, constructorParameters, excludes, processingFunction, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of a new destination object
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapTo<TDestination>(this object source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            if(source == null)
                return default(TDestination);

            Type type = source.GetType();

            MethodInfo method = typeof(Mapper).GetMethods().Where(x => x.Name == "Map" && x.GetParameters().Length == 5).First();
            MethodInfo generic = method.MakeGenericMethod(new Type[] { type, typeof(TDestination) });

            return (TDestination)generic.Invoke(null, new object[] { source, constructorParameters, excludes, processingFunction, processingExcludes });
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to any correspondingly-named 
        /// properties of an existing destination object of type TDestination which are prefixed with the source type name
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapToPrefixed<TDestination>(this object source)
        {
            return MapToPrefixed<TDestination>(source, null, null, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to any correspondingly-named 
        /// properties of an existing destination object of type TDestination which are prefixed with the source type name
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapToPrefixed<TDestination>(this object source, object[] constructorParameters)
        {
            return MapToPrefixed<TDestination>(source, constructorParameters, null, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to any correspondingly-named 
        /// properties of an existing destination object of type TDestination which are prefixed with the source type name
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapToPrefixed<TDestination>(this object source, object[] constructorParameters, List<string> excludes)
        {
            return MapToPrefixed<TDestination>(source, constructorParameters, excludes, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to any correspondingly-named 
        /// properties of an existing destination object of type TDestination which are prefixed with the source type name
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapToPrefixed<TDestination>(this object source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction)
        {
            return MapToPrefixed<TDestination>(source, constructorParameters, excludes, processingFunction, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to any correspondingly-named 
        /// properties of an existing destination object of type TDestination which are prefixed with the source type name
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapToPrefixed<TDestination>(this object source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            if(source == null)
                return default(TDestination);

            Type type = source.GetType();

            MethodInfo method = typeof(Mapper).GetMethods().Where(x => x.Name == "MapToPrefixed" && x.GetParameters().Length == 5).First();
            MethodInfo generic = method.MakeGenericMethod(new Type[] { type, typeof(TDestination) });

            return (TDestination)generic.Invoke(null, new object[] { source, constructorParameters, excludes, processingFunction, processingExcludes });
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapFromPrefixed<TDestination>(this object source)
        {
            return MapFromPrefixed<TDestination>(source, null, null, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapFromPrefixed<TDestination>(this object source, object[] constructorParameters)
        {
            return MapFromPrefixed<TDestination>(source, constructorParameters, null, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapFromPrefixed<TDestination>(this object source, object[] constructorParameters, List<string> excludes)
        {
            return MapFromPrefixed<TDestination>(source, constructorParameters, excludes, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapFromPrefixed<TDestination>(this object source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction)
        {
            return MapFromPrefixed<TDestination>(source, constructorParameters, excludes, processingFunction, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapFromPrefixed<TDestination>(this object source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            if(source == null)
                return default(TDestination);

            Type type = source.GetType();

            MethodInfo method = typeof(Mapper).GetMethods().Where(x => x.Name == "MapFromPrefixed" && x.GetParameters().Length == 5).First();
            MethodInfo generic = method.MakeGenericMethod(new Type[] { type, typeof(TDestination) });

            return (TDestination)generic.Invoke(null, new object[] { source, constructorParameters, excludes, processingFunction, processingExcludes });
        }

        /// <summary>
        /// Map all the properties of every object in a source list to the correspondingly-named 
        /// properties of objects in a new destination list
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source list</param>
        /// <param name="destination">Destination list</param>
        /// <returns>New object of type List<TDestination></returns>
        public static List<TDestination> MapToList<TDestination>(this object source)
        {
            return MapToList<TDestination>(source, null, new List<string>(), null, new List<string>());
        }

        /// <summary>
        /// Map all the properties of every object in a source list to the correspondingly-named 
        /// properties of objects in a new destination list
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source list</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <returns>New object of type List<TDestination></returns>
        public static List<TDestination> MapToList<TDestination>(this object source, object[] constructorParameters)
        {
            return MapToList<TDestination>(source, constructorParameters, new List<string>(), null, new List<string>());
        }

        /// <summary>
        /// Map all the properties of every object in a source list to the correspondingly-named 
        /// properties of objects in a new destination list
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source list</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <returns>New object of type List<TDestination></returns>
        public static List<TDestination> MapToList<TDestination>(this object source, object[] constructorParameters, List<string> excludes)
        {
            return MapToList<TDestination>(source, constructorParameters, excludes, null, new List<string>());
        }

        /// <summary>
        /// Map all the properties of every object in a source list to the correspondingly-named 
        /// properties of objects in a new destination list
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source list</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <returns>New object of type List<TDestination></returns>
        public static List<TDestination> MapToList<TDestination>(this object source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction)
        {
            return MapToList<TDestination>(source, constructorParameters, excludes, processingFunction, new List<string>());
        }

        /// <summary>
        /// Map all the properties of every object in a source list to the correspondingly-named 
        /// properties of objects in a new destination list
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source list</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        /// <returns>New object of type List<TDestination></returns>
        public static List<TDestination> MapToList<TDestination>(this object source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            if(source == null)
                return default(List<TDestination>);

            Type type = source.GetType();

            MethodInfo method = typeof(Mapper).GetMethods().Where(x => x.Name == "MapList" && x.GetParameters().Length == 5).First();
            MethodInfo generic = method.MakeGenericMethod(new Type[] { type.GetGenericArguments()[0], typeof(TDestination) });

            return (List<TDestination>)generic.Invoke(null, new object[] { source, constructorParameters, excludes, processingFunction, processingExcludes });
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of an existing destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        public static void MapTo(this object source, object destination)
        {
            MapTo(source, destination, new List<string>(), null, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of an existing destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        public static void MapTo(this object source, object destination, List<string> excludes)
        {
            MapTo(source, destination, excludes, null, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of an existing destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        public static void MapTo(this object source, object destination, List<string> excludes, Func<object, object> processingFunction)
        {
            MapTo(source, destination, excludes, processingFunction, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of an existing destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        public static void MapTo(this object source, object destination, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            Type sourceType = source.GetType();
            Type destinationType = destination.GetType();

            // Get the particular overload of CopyProperties with five parameters
            MethodInfo method = typeof(Mapper).GetMethods().Where(x => x.Name == "Map" && x.GetParameters().Length == 5).Skip(1).First();
            MethodInfo generic = method.MakeGenericMethod(new Type[] { sourceType, destinationType });

            //MethodInfo methods = typeof(ObjectMapper).GetMethods().Where(x => x.Name == "Map");
            //MethodInfo generic = method.MakeGenericMethod(new Type[] { sourceType, destinationType });

            generic.Invoke(null, new object[] { source, destination, excludes, processingFunction, processingExcludes });
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of an existing destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        public static void MapPrefixedTo(this object source, object destination)
        {
            MapPrefixedTo(source, destination, null, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of an existing destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        public static void MapPrefixedTo(this object source, object destination, List<string> excludes)
        {
            MapPrefixedTo(source, destination, excludes, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of an existing destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        public static void MapPrefixedTo(this object source, object destination, List<string> excludes, Func<object, object> processingFunction)
        {
            MapPrefixedTo(source, destination, excludes, processingFunction, null);
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of an existing destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        public static void MapPrefixedTo(this object source, object destination, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            Type sourceType = source.GetType();
            Type destinationType = destination.GetType();

            // Get the particular overload of CopyProperties with five parameters
            MethodInfo method = typeof(Mapper).GetMethods().Where(x => x.Name == "MapFromPrefixed" && x.GetParameters().Length == 5).Skip(1).First();
            MethodInfo generic = method.MakeGenericMethod(new Type[] { sourceType, destinationType });

            //MethodInfo methods = typeof(ObjectMapper).GetMethods().Where(x => x.Name == "Map");
            //MethodInfo generic = method.MakeGenericMethod(new Type[] { sourceType, destinationType });

            generic.Invoke(null, new object[] { source, destination, excludes, processingFunction, processingExcludes });
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of an existing destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        public static void MapToPrefixed(this object source, object destination)
        {
            MapToPrefixed(source, destination, null, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of an existing destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        public static void MapToPrefixed(this object source, object destination, List<string> excludes)
        {
            MapToPrefixed(source, destination, excludes, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of an existing destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        public static void MapToPrefixed(this object source, object destination, List<string> excludes, Func<object, object> processingFunction)
        {
            MapToPrefixed(source, destination, excludes, processingFunction, null);
        }

        /// <summary>
        /// Map all the property values of a source object to the correspondingly-named 
        /// properties of an existing destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        public static void MapToPrefixed(this object source, object destination, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            Type sourceType = source.GetType();
            Type destinationType = destination.GetType();

            // Get the particular overload of CopyProperties with five parameters
            MethodInfo method = typeof(Mapper).GetMethods().Where(x => x.Name == "MapToPrefixed" && x.GetParameters().Length == 5).Skip(1).First();
            MethodInfo generic = method.MakeGenericMethod(new Type[] { sourceType, destinationType });

            //MethodInfo methods = typeof(ObjectMapper).GetMethods().Where(x => x.Name == "Map");
            //MethodInfo generic = method.MakeGenericMethod(new Type[] { sourceType, destinationType });

            generic.Invoke(null, new object[] { source, destination, excludes, processingFunction, processingExcludes });
        }
    }
}
