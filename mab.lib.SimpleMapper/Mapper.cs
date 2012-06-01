using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace mab.lib.SimpleMapper
{
    public static class Mapper
    {
        // Declare a list of types we want to try and map (so our mapper
        // desn't attempt to map properties which are complex types)
        private static List<object> _types = new List<object>
        {
            typeof(string),
            typeof(int),
            typeof(bool),
            typeof(decimal),
            typeof(double),
            typeof(short),
            typeof(System.DateTime),
            typeof(System.Nullable<int>),
            typeof(System.Nullable<bool>),
            typeof(System.Nullable<decimal>),
            typeof(System.Nullable<double>),
            typeof(System.Nullable<short>),
            typeof(System.Nullable<System.DateTime>)
        };

        /// <summary>
        /// Map a source object instance to a new destination object instance
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object instance</param>
        /// <returns>Object of type TDestination</returns>
        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            if (source == null) return default(TDestination);

            var destination = Activator.CreateInstance<TDestination>();
            return Map<TSource, TDestination>(source, destination);
        }

        /// <summary>
        /// Map a source object instance to a new destination object instance,
        /// passing constructor parameters to the destination instance
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object instance</param>
        /// <param name="args">Constructor arguments for destination object</param>
        /// <returns>Object of type TDestination</returns>
        public static TDestination Map<TSource, TDestination>(TSource source, params object[] args)
        {
            if(source == null)
            {
                return default(TDestination);
            }

            var destination = (TDestination)Activator.CreateInstance(typeof(TDestination), args);
            return Map<TSource, TDestination>(source, destination);
        }

        /// <summary>
        /// Map a source object instance to an existing destination object instance
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object instance</param>
        /// <param name="destination">Destination object instance</param>
        /// <returns>Object of type TDestination</returns>
        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            if (source == null) return default(TDestination);

            var sourceType = source.GetType();
            var destinationType = destination.GetType();

            var sourceProperties = sourceType.GetProperties();
            var destinationProperties = destinationType.GetProperties();

            foreach (var property in sourceProperties)
            {
                var pType = property.PropertyType;

                if (_types.Contains(pType))
                {
                    var destinationProperty = destinationProperties.FirstOrDefault(x => x.Name == property.Name);

                    if (destinationProperty != null)
                    {
                        try
                        {
                            var val = property.GetValue(source, null);
                            destinationProperty.SetValue(destination, val, null);
                        }
                        catch (ArgumentException e)
                        {
                            if (e.Message != "Property set method not found.")
                                throw e;
                        }
                    }
                }
            }

            return destination;
        }

        /// <summary>
        /// Map a source list of object instances to a new destination list of object instances
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source list of object instances</param>
        /// <returns>List of objects of type TDestination</returns>
        public static List<TDestination> MapList<TSource, TDestination>(List<TSource> source)
        {
            if (source == null) return default(List<TDestination>);

            var destination = new List<TDestination>();

            foreach (var item in source)
                destination.Add(Map<TSource, TDestination>(item));

            return destination;
        }

        /// <summary>
        /// Map a source list of object instances to a new destination list of object instances,
        /// passing constructor parameters to the destination list object instances
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source list of object instances</param
        /// <param name="args">Constructor arguments for destination list objects</param>
        /// <returns>List of objects of type TDestination</returns>
        public static List<TDestination> MapList<TSource, TDestination>(List<TSource> source, params object[] args)
        {
            if(source == null)
            {
                return default(List<TDestination>);
            }

            var destination = new List<TDestination>();

            foreach(var item in source)
            {
                destination.Add(Map<TSource, TDestination>(item, args));
            }

            return destination;
        }

        /// <summary>
        /// Copy all property values from one object instance to another
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object instance</param>
        /// <param name="destination">Destination object instance</param>
        public static void CopyProperties<TSource, TDestination>(TSource source, TDestination destination)
        {
            CopyProperties<TSource, TDestination>(source, destination, new List<string>(), null, new List<string>());
        }

        /// <summary>
        /// Copy property values from one object instance to another, excluding
        /// those contained in a list of property names to skip
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object instance</param>
        /// <param name="destination">Destination object instance</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        public static void CopyProperties<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes)
        {
            CopyProperties<TSource, TDestination>(source, destination, excludes, null, new List<string>());   
        }

        /// <summary>
        /// Set property values in a destination object instance to the result of a function which
        /// processes the value of the corresponding property in the source object instance, excluding
        /// those contained in a list of property names to skip
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object instance</param>
        /// <param name="destination">Destination object instance</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction"></param>
        public static void CopyProperties<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction)
        {
            CopyProperties<TSource, TDestination>(source, destination, excludes, processingFunction, new List<string>());
        }

        public static void CopyProperties<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            var sourceType = source.GetType();
            var destinationType = destination.GetType();

            var sourceProperties = sourceType.GetProperties();
            var destinationProperties = destinationType.GetProperties();

            foreach (var property in sourceProperties)
            {
                var doMapping = true;

                // Don't map properties where the property name matches any of the exclude regular expressions
                foreach (var exclude in excludes)
                    if (Regex.IsMatch(property.Name, exclude, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                        doMapping = false;

                if (doMapping)
                {
                    var pType = property.PropertyType;

                    if (_types.Contains(pType))
                    {
                        var destinationProperty = destinationProperties.FirstOrDefault(x => x.Name == property.Name);

                        if (destinationProperty != null)
                        {
                            try
                            {
                                var val = property.GetValue(source, null);

                                if (processingFunction != null && !processingExcludes.Contains(property.Name))
                                    val = processingFunction(val);

                                destinationProperty.SetValue(destination, val, null);
                            }
                            catch (ArgumentException e)
                            {
                                if (e.Message != "Property set method not found.")
                                    throw e;
                            }
                        }
                    }
                }
            }
        }
    }
}
