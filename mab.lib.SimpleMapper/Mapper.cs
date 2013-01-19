/*
The MIT License (MIT)
Copyright (c) 2013 Mark Ashley Bell (http://markashleybell.com)
 
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Reflection;

namespace mab.lib.SimpleMapper
{
    public static class Mapper
    {
        // Tuple-keyed dictionary to allow us to look up custom mappings 
        // based on the combination of source and destination type
        private static Dictionary<Tuple<Type, Type>, object> _maps = new Dictionary<Tuple<Type, Type>, object>();

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
            _maps.Add(Tuple.Create(typeof(TSource), typeof(TDestination)), map);
        }

        /// <summary>
        /// Clear all custom mappings
        /// </summary>
        public static void ClearMappings()
        {
            _maps.Clear();
        }

        /// <summary>
        /// Map a source object of type TSource to a new object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>New object of type TDestination</returns>
        private static TDestination Map<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class
        {
            if (source == null)
                return default(TDestination);

            // Create a new instance of the destination type
            var destination = Activator.CreateInstance<TDestination>();

            Map<TSource, TDestination>(source, destination);

            return destination;
        }

        /// <summary>
        /// Map a source object of type TSource to an existing object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        private static void Map<TSource, TDestination>(TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            var key = Tuple.Create(typeof(TSource), typeof(TDestination));

            var map = (_maps.ContainsKey(key)) ? _maps[key] as Action<TSource, TDestination> : null;

            if (map == null)
            {
                // There's no specific mapping set up, so we'll just do it by convention
                var sourceProperties = source.GetType().GetProperties();
                var destinationProperties = destination.GetType().GetProperties();

                // Loop through the properties of the source object
                foreach (var property in sourceProperties)
                {
                    // Try and find a matching property of the destination type (match on name and type)
                    var destinationProperty = destinationProperties.FirstOrDefault(x => {
                        return x.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase)
                            && (x.PropertyType == property.PropertyType) || (x.PropertyType.IsEnum && property.PropertyType.IsEnum)
                            && x.CanWrite;
                    });

                    // If the destination type has a matching property
                    if (destinationProperty != null)
                    {
                        // Update the destination property with the value of the source property
                        var val = property.GetValue(source, null);
                        destinationProperty.SetValue(destination, val, null);
                    }
                }

                // TODO: Generate delegate code using CSharpCodeProvider and add the delegate to custom mappings collection
                // This will avoid reflection on every call to the mapper
            }
            else
            {
                // Map using the stored delegate for this source/destination type combination
                map(source, destination);
            }
        }

        /// <summary>
        /// Map a list of objects of type TSource to a new list of objects of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source list</param>
        /// <returns>New list of objects of type TDestination</returns>
        private static List<TDestination> MapList<TSource, TDestination>(List<TSource> source)
            where TSource : class
            where TDestination : class
        {
            // TODO: Should throw exception, surely. Was there a reason I did this in the previous version? 
            //if (source == null)
            //    return default(List<TDestination>);

            var destination = new List<TDestination>();

            foreach (var item in source)
                destination.Add(Map<TSource, TDestination>(item));

            return destination;
        }

        /// <summary>
        /// Map a source object to a destination object
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        public static void MapTo(this object source, object destination)
        {
            // Use reflection to get the Map<TSource, TDestination>(TSource source, TDestination destination) method
            MethodInfo method = typeof(Mapper).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                                              .Where(x => x.Name == "Map" && x.GetParameters().Length == 2)
                                              .First();

            // Get the types of the source and destination objects and pass them into the method as type parameters
            MethodInfo generic = method.MakeGenericMethod(new Type[] { source.GetType(), destination.GetType() });

            // Invoke the Map method, passing in the source and destination objects as method parameters
            generic.Invoke(null, new object[] { source, destination });
        }

        /// <summary>
        /// Map a source object to a new destination object of type TDestination
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapTo<TDestination>(this object source)
            where TDestination : class
        {
            // Use reflection to get the Map<TSource, TDestination>(TSource source) method
            MethodInfo method = typeof(Mapper).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                                              .Where(x => x.Name == "Map" && x.GetParameters().Length == 1)
                                              .First();

            // Get the type of the source objects and pass it into the method as a type parameter,
            // along with the type of the new destination object
            MethodInfo generic = method.MakeGenericMethod(new Type[] { source.GetType(), typeof(TDestination) });

            // Invoke the Map method, passing in the source object as a method parameter, and 
            // return the resulting object of type TDestination
            return (TDestination)generic.Invoke(null, new object[] { source });
        }

        /// <summary>
        /// Map a list of source objects to a new list of objects of type TDestination
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source list</param>
        /// <returns>New list of objects of type TDestination</returns>
        public static List<TDestination> MapToList<TDestination>(this object source)
            where TDestination : class
        {
            // Use reflection to get the MapList<TSource, TDestination>(TSource source) method
            MethodInfo method = typeof(Mapper).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                                              .Where(x => x.Name == "MapList" && x.GetParameters().Length == 1)
                                              .First();

            // Get the type argument of the source list and pass it into the method as a type parameter,
            // along with the type of the new destination list
            MethodInfo generic = method.MakeGenericMethod(new Type[] { source.GetType().GetGenericArguments()[0], typeof(TDestination) });

            // Invoke the MapList method, passing in the source list as a method parameter, and 
            // return the resulting list of objects of type TDestination
            return (List<TDestination>)generic.Invoke(null, new object[] { source });
        }
    }
}
