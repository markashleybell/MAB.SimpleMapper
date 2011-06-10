using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mab.lib.SimpleMapper
{
    public static class Mapper
    {
        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            if (source == null) return default(TDestination);

            var destination = Activator.CreateInstance<TDestination>();
            return Map<TSource, TDestination>(source, destination);
        }

        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            if (source == null) return default(TDestination);

            var sourceType = source.GetType();
            var destinationType = destination.GetType();

            var sourceProperties = sourceType.GetProperties();
            var destinationProperties = destinationType.GetProperties();

            var types = new List<object>
            {
                typeof(string),
                typeof(int),
                typeof(bool),
                typeof(decimal),
                typeof(System.DateTime),
                typeof(System.Nullable<int>),
                typeof(System.Nullable<decimal>),
                typeof(System.Nullable<System.DateTime>)
            };

            foreach (var property in sourceProperties)
            {
                var pType = property.PropertyType;

                if (types.Contains(pType))
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
                destination.Add(Mapper.Map<TSource, TDestination>(item));

            return destination;
        }
    }
}
