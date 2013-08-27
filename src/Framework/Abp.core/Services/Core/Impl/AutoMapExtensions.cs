using System.Collections.Generic;

namespace Abp.Services.Core.Impl
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
    }
}