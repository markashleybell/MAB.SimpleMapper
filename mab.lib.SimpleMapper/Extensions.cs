using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace mab.lib.SimpleMapper
{
    public static class Extensions
    {
        /// <summary>
        /// Generic extension which can be called on any object, mapping its property values
        /// to any properties of a new destination type instance with the same names
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object instance</param>
        /// <returns>New instance of type TDestination</returns>
        public static TDestination MapTo<TDestination>(this object source)
        {
            if(source == null)
                return default(TDestination);
            Type type = source.GetType();

            MethodInfo method = typeof(Mapper).GetMethods().Where(x => x.Name == "Map").First();
            MethodInfo generic = method.MakeGenericMethod(new Type[] { type, typeof(TDestination) });

            return (TDestination)generic.Invoke(null, new object[] { source });
        }

        /// <summary>
        /// Generic extension which can be called on a list of any type, mapping each instance's property values
        /// to any properties of new destination type instance with the same names and adding each to a new list
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source list of object instances</param>
        /// <returns>New list of objects of type TDestination</returns>
        public static List<TDestination> MapToList<TDestination>(this object source)
        {
            if(source == null)
                return default(List<TDestination>);

            Type type = source.GetType();

            MethodInfo method = typeof(Mapper).GetMethods().Where(x => x.Name == "MapList").First();
            MethodInfo generic = method.MakeGenericMethod(new Type[] { type.GetGenericArguments()[0], typeof(TDestination) });

            return (List<TDestination>)generic.Invoke(null, new object[] { source });
        }
    }
}
