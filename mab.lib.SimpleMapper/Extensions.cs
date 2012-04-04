using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace mab.lib.SimpleMapper
{
    public static class Extensions
    {
        public static TDestination MapTo<TDestination>(this object source)
        {
            if(source == null)
                return default(TDestination);
            Type type = source.GetType();

            MethodInfo method = typeof(Mapper).GetMethods().Where(x => x.Name == "Map").First();
            MethodInfo generic = method.MakeGenericMethod(new Type[] { type, typeof(TDestination) });

            return (TDestination)generic.Invoke(null, new object[] { source });
        }

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
