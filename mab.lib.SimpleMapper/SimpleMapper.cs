using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace mab.lib.SimpleMapper
{
    public static class Mapper
    {
        private static List<object> _types = new List<object>
        {
            typeof(string),
            typeof(int),
            typeof(bool),
            typeof(decimal),
            typeof(System.DateTime),
            typeof(System.Nullable<int>),
            typeof(System.Nullable<bool>),
            typeof(System.Nullable<decimal>),
            typeof(System.Nullable<System.DateTime>)
        };

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            if (source == null) return default(TDestination);

            var destination = Activator.CreateInstance<TDestination>();
            return Map<TSource, TDestination>(source, destination);
        }

        public static TDestination Map<TSource, TDestination>(TSource source, params object[] args)
        {
            if(source == null)
            {
                return default(TDestination);
            }

            var destination = (TDestination)Activator.CreateInstance(typeof(TDestination), args);
            return Map<TSource, TDestination>(source, destination);
        }

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

        public static List<TDestination> MapList<TSource, TDestination>(List<TSource> source)
        {
            if (source == null) return default(List<TDestination>);

            var destination = new List<TDestination>();

            foreach (var item in source)
                destination.Add(Map<TSource, TDestination>(item));

            return destination;
        }

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

        public static void CopyProperties<TSource, TDestination>(TSource source, TDestination destination)
        {
            // By default we don't want to map any property ending with "ID" (leave this to EF)
            CopyProperties<TSource, TDestination>(source, destination, new List<string> { "^.*ID$" }, null, new List<string> { });
        }

        public static void CopyProperties<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes)
        {
            CopyProperties<TSource, TDestination>(source, destination, excludes, null, new List<string> { });
        }

        public static void CopyProperties<TSource, TDestination>(TSource source, TDestination destination, List<string> excludes, Func<object, object> processingFunction)
        {
            CopyProperties<TSource, TDestination>(source, destination, excludes, processingFunction, new List<string> { });
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
