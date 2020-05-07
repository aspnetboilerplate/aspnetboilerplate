using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Abp.AspNetCore.Mvc.Results.Caching
{
    [Obsolete]
    public interface IClientCacheAttribute
    {
        void Apply(ResultExecutingContext context);
    }
}