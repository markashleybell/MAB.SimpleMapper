using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mab.lib.SimpleMapper
{
    public static class Extensions
    {
        public static TDestination MapTo<TDestination>(this object source)
        {
            return Mapper.Map<dynamic, TDestination>(source);
        }
    }
}
