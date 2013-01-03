using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace mab.lib.SimpleMapper
{
    public static class Mapper
    {
        private static List<object> _types = new List<object> {
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
        /// Map all the property values of a source object of type TSource to the correspondingly-named 
        /// properties of a new destination object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            return Map<TSource, TDestination>(source, null, new List<string>(), null, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to the correspondingly-named 
        /// properties of a new destination object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination Map<TSource, TDestination>(TSource source, object[] constructorParameters)
        {
            return Map<TSource, TDestination>(source, constructorParameters, new List<string>(), null, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to the correspondingly-named 
        /// properties of a new destination object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination Map<TSource, TDestination>(TSource source, object[] constructorParameters, List<string> excludes)
        {
            return Map<TSource, TDestination>(source, constructorParameters, excludes, null, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to the correspondingly-named 
        /// properties of a new destination object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination Map<TSource, TDestination>(TSource source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction)
        {
            return Map<TSource, TDestination>(source, constructorParameters, excludes, processingFunction, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to the correspondingly-named 
        /// properties of a new destination object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination Map<TSource, TDestination>(TSource source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            if(source == null)
            {
                return default(TDestination);
            }

            TDestination destination;
            if(constructorParameters == null)
                destination = Activator.CreateInstance<TDestination>();
            else
                destination = (TDestination)Activator.CreateInstance(typeof(TDestination), constructorParameters);

            Map<TSource, TDestination>(source, destination, excludes, processingFunction, processingExcludes);
            return destination;
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to any correspondingly-named 
        /// properties of an existing destination object of type TDestination which are prefixed with the source type name
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapToPrefixed<TSource, TDestination>(TSource source)
        {
            return MapToPrefixed<TSource, TDestination>(source, null, null, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to any correspondingly-named 
        /// properties of an existing destination object of type TDestination which are prefixed with the source type name
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapToPrefixed<TSource, TDestination>(TSource source, object[] constructorParameters)
        {
            return MapToPrefixed<TSource, TDestination>(source, constructorParameters, null, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to any correspondingly-named 
        /// properties of an existing destination object of type TDestination which are prefixed with the source type name
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapToPrefixed<TSource, TDestination>(TSource source, object[] constructorParameters, List<string> excludes)
        {
            return MapToPrefixed<TSource, TDestination>(source, constructorParameters, excludes, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to any correspondingly-named 
        /// properties of an existing destination object of type TDestination which are prefixed with the source type name
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapToPrefixed<TSource, TDestination>(TSource source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction)
        {
            return MapToPrefixed<TSource, TDestination>(source, constructorParameters, excludes, processingFunction, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to the correspondingly-named 
        /// properties of a new destination object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapToPrefixed<TSource, TDestination>(TSource source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            if(source == null)
            {
                return default(TDestination);
            }

            TDestination destination;
            if(constructorParameters == null)
                destination = Activator.CreateInstance<TDestination>();
            else
                destination = (TDestination)Activator.CreateInstance(typeof(TDestination), constructorParameters);

            MapToPrefixed<TSource, TDestination>(source, destination, excludes, processingFunction, processingExcludes);
            return destination;
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapFromPrefixed<TSource, TDestination>(TSource source)
        {
            return MapFromPrefixed<TSource, TDestination>(source, null, null, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapFromPrefixed<TSource, TDestination>(TSource source, object[] constructorParameters)
        {
            return MapFromPrefixed<TSource, TDestination>(source, constructorParameters, null, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapFromPrefixed<TSource, TDestination>(TSource source, object[] constructorParameters, List<string> excludes)
        {
            return MapFromPrefixed<TSource, TDestination>(source, constructorParameters, excludes, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapFromPrefixed<TSource, TDestination>(TSource source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction)
        {
            return MapFromPrefixed<TSource, TDestination>(source, constructorParameters, excludes, processingFunction, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        /// <returns>New object of type TDestination</returns>
        public static TDestination MapFromPrefixed<TSource, TDestination>(TSource source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            if(source == null)
            {
                return default(TDestination);
            }

            TDestination destination;
            if(constructorParameters == null)
                destination = Activator.CreateInstance<TDestination>();
            else
                destination = (TDestination)Activator.CreateInstance(typeof(TDestination), constructorParameters);

            MapFromPrefixed<TSource, TDestination>(source, destination, excludes, processingFunction, processingExcludes);
            return destination;
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to the correspondingly-named 
        /// properties of an existing destination object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <returns>New object of type TDestination</returns>
        public static void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            Map<TSource, TDestination>(source, destination, new List<string>(), null, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to the correspondingly-named 
        /// properties of an existing destination object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <returns>New object of type TDestination</returns>
        public static void Map<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes)
        {
            Map<TSource, TDestination>(source, destination, excludes, null, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to the correspondingly-named 
        /// properties of an existing destination object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <returns>New object of type TDestination</returns>
        public static void Map<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction)
        {
            Map<TSource, TDestination>(source, destination, excludes, processingFunction, new List<string>());
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to the correspondingly-named 
        /// properties of an existing destination object of type TDestination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        /// <returns>New object of type TDestination</returns>
        public static void Map<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            var sourceType = source.GetType();
            var destinationType = destination.GetType();

            var sourceProperties = sourceType.GetProperties();
            var destinationProperties = destinationType.GetProperties();

            foreach(var property in sourceProperties)
            {
                var doMapping = true;

                // Don't map properties where the property name matches any of the exclude regular expressions
                if (excludes != null)
                {
                    foreach (var exclude in excludes)
                        if (Regex.IsMatch(property.Name, exclude, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                            doMapping = false;
                }

                if(doMapping)
                {
                    var pType = property.PropertyType;

                    if(_types.Contains(pType))
                    {
                        var destinationProperty = destinationProperties.FirstOrDefault(x => x.Name == property.Name);

                        if(destinationProperty != null)
                        {
                            try
                            {
                                var val = property.GetValue(source, null);

                                if(processingFunction != null && (processingExcludes == null || !processingExcludes.Contains(property.Name)))
                                    val = processingFunction(val);

                                destinationProperty.SetValue(destination, val, null);
                            }
                            catch(ArgumentException e)
                            {
                                if(e.Message != "Property set method not found.")
                                    throw e;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to any correspondingly-named 
        /// properties of an existing destination object of type TDestination which are prefixed with the source type name
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <returns>New object of type TDestination</returns>
        public static void MapToPrefixed<TSource, TDestination>(TSource source, TDestination destination)
        {
            MapToPrefixed<TSource, TDestination>(source, destination, null, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to any correspondingly-named 
        /// properties of an existing destination object of type TDestination which are prefixed with the source type name
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <returns>New object of type TDestination</returns>
        public static void MapToPrefixed<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes)
        {
            MapToPrefixed<TSource, TDestination>(source, destination, excludes, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to any correspondingly-named 
        /// properties of an existing destination object of type TDestination which are prefixed with the source type name
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <returns>New object of type TDestination</returns>
        public static void MapToPrefixed<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction)
        {
            MapToPrefixed<TSource, TDestination>(source, destination, excludes, processingFunction, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource to any correspondingly-named 
        /// properties of an existing destination object of type TDestination which are prefixed with the source type name
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        /// <returns>New object of type TDestination</returns>
        public static void MapToPrefixed<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            var sourceType = source.GetType();
            var destinationType = destination.GetType();

            var sourceProperties = sourceType.GetProperties();
            var destinationProperties = destinationType.GetProperties();

            foreach(var property in sourceProperties)
            {
                var doMapping = true;

                // Don't map properties where the property name matches any of the exclude regular expressions
                if (excludes != null)
                {
                    foreach (var exclude in excludes)
                        if (Regex.IsMatch(property.Name, exclude, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                            doMapping = false;
                }

                if(doMapping)
                {
                    var pType = property.PropertyType;

                    if(_types.Contains(pType))
                    {
                        var destinationProperty = destinationProperties.FirstOrDefault(x => x.Name == sourceType.Name + "_" + property.Name);

                        if(destinationProperty != null)
                        {
                            try
                            {
                                var val = property.GetValue(source, null);

                                if(processingFunction != null && (processingExcludes == null || !processingExcludes.Contains(property.Name)))
                                    val = processingFunction(val);

                                destinationProperty.SetValue(destination, val, null);
                            }
                            catch(ArgumentException e)
                            {
                                if(e.Message != "Property set method not found.")
                                    throw e;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <returns>New object of type TDestination</returns>
        public static void MapFromPrefixed<TSource, TDestination>(TSource source, TDestination destination)
        {
            MapFromPrefixed<TSource, TDestination>(source, destination, null, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <returns>New object of type TDestination</returns>
        public static void MapFromPrefixed<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes)
        {
            MapFromPrefixed<TSource, TDestination>(source, destination, excludes, null, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <returns>New object of type TDestination</returns>
        public static void MapFromPrefixed<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction)
        {
            MapFromPrefixed<TSource, TDestination>(source, destination, excludes, processingFunction, null);
        }

        /// <summary>
        /// Map all the property values of a source object of type TSource which are prefixed with the type name
        /// of TDestination (e.g TDestination_ID) to any correspondingly-named properties of an existing destination 
        /// object of type TDestination 
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        /// <returns>New object of type TDestination</returns>
        public static void MapFromPrefixed<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            var sourceType = source.GetType();
            var destinationType = destination.GetType();

            var sourceProperties = sourceType.GetProperties();
            var destinationProperties = destinationType.GetProperties();

            foreach(var property in sourceProperties.Where(p => p.Name.StartsWith(destinationType.Name + "_")))
            {
                var doMapping = true;

                // Don't map properties where the property name matches any of the exclude regular expressions
                if (excludes != null)
                {
                    foreach (var exclude in excludes)
                        if (Regex.IsMatch(property.Name, exclude, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                            doMapping = false;
                }

                if(doMapping)
                {
                    var pType = property.PropertyType;

                    if(_types.Contains(pType))
                    {
                        var destinationPropertyName = property.Name.Substring(property.Name.IndexOf("_") + 1);

                        var destinationProperty = destinationProperties.FirstOrDefault(x => x.Name == destinationPropertyName);

                        if(destinationProperty != null)
                        {
                            try
                            {
                                var val = property.GetValue(source, null);

                                if(processingFunction != null && (processingExcludes == null || !processingExcludes.Contains(property.Name)))
                                    val = processingFunction(val);

                                destinationProperty.SetValue(destination, val, null);
                            }
                            catch(ArgumentException e)
                            {
                                if(e.Message != "Property set method not found.")
                                    throw e;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Map all the properties of every object in a source list to the correspondingly-named 
        /// properties of objects in a new destination list
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>New object of type List<TDestination></returns>
        public static List<TDestination> MapList<TSource, TDestination>(List<TSource> source)
        {
            return MapList<TSource, TDestination>(source, null, new List<string>(), null, new List<string>());
        }

        /// <summary>
        /// Map all the properties of every object in a source list to the correspondingly-named 
        /// properties of objects in a new destination list
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <returns>New object of type List<TDestination></returns>
        public static List<TDestination> MapList<TSource, TDestination>(List<TSource> source, object[] constructorParameters)
        {
            return MapList<TSource, TDestination>(source, constructorParameters, new List<string>(), null, new List<string>());
        }

        /// <summary>
        /// Map all the properties of every object in a source list to the correspondingly-named 
        /// properties of objects in a new destination list
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <returns>New object of type List<TDestination></returns>
        public static List<TDestination> MapList<TSource, TDestination>(List<TSource> source, object[] constructorParameters, List<string> excludes)
        {
            return MapList<TSource, TDestination>(source, constructorParameters, excludes, null, new List<string>());
        }

        /// <summary>
        /// Map all the properties of every object in a source list to the correspondingly-named 
        /// properties of objects in a new destination list
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <returns>New object of type List<TDestination></returns>
        public static List<TDestination> MapList<TSource, TDestination>(List<TSource> source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction)
        {
            return MapList<TSource, TDestination>(source, constructorParameters, excludes, processingFunction, new List<string>());
        }

        /// <summary>
        /// Map all the properties of every object in a source list to the correspondingly-named 
        /// properties of objects in a new destination list
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="constructorParameters">Constructor parameters to pass to the newly instantiated objects</param>
        /// <param name="excludes">A list of property names for which values should not be copied</param>
        /// <param name="processingFunction">A function to apply to each property value before it is assigned to the destination property</param>
        /// <param name="processingExcludes">A list of property names for which the processing function should not be applied</param>
        /// <returns>New object of type List<TDestination></returns>
        public static List<TDestination> MapList<TSource, TDestination>(List<TSource> source, object[] constructorParameters, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            if(source == null)
            {
                return default(List<TDestination>);
            }

            var destination = new List<TDestination>();

            foreach(var item in source)
            {
                destination.Add(Map<TSource, TDestination>(item, constructorParameters, excludes, processingFunction, processingExcludes));
            }

            return destination;
        }

        [Obsolete("CopyProperties<TSource, TDestination>(TSource source, TDestination destination) is deprecated, please use Map<TSource, TDestination>(TSource source, TDestination destination) instead.", true)]
        public static void CopyProperties<TSource, TDestination>(TSource source, TDestination destination)
        {
            Map<TSource, TDestination>(source, destination, new List<string>(), null, new List<string>());
        }

        [Obsolete("CopyProperties<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes) is deprecated, please use Map<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes) instead.", true)]
        public static void CopyProperties<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes)
        {
            Map<TSource, TDestination>(source, destination, excludes, null, new List<string>());   
        }

        [Obsolete("CopyProperties<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction) is deprecated, please use Map<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction) instead.", true)]
        public static void CopyProperties<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction)
        {
            Map<TSource, TDestination>(source, destination, excludes, processingFunction, new List<string>());
        }

        [Obsolete("CopyProperties<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes) is deprecated, please use Map<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes) instead.", true)]
        public static void CopyProperties<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction, List<string> processingExcludes)
        {
            Map<TSource, TDestination>(source, destination, excludes, processingFunction, processingExcludes);
        }
    }
}
