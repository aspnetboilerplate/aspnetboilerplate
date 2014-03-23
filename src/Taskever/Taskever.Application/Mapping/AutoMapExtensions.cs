using System.Collections.Generic;

namespace Abp.Mapping
{
    /// <summary>
    /// TODO: It's not so good to be depended to a mapping library!
    /// </summary>
    public static class AutoMapExtensions
    {
        public static IList<TD> MapIList<TS, TD>(this IList<TS> items)
        {
            return AutoMapper.Mapper.Map<IList<TD>>(items); //TODO: ???            
        }

        public static T MapTo<T>(this object obj)
        {
            if (obj == null)
            {
                return default(T);
            }

            return AutoMapper.Mapper.Map<T>(obj);
        }
    }
}