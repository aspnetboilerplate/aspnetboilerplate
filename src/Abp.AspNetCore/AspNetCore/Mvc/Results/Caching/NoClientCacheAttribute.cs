using Abp.AspNetCore.Mvc.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results.Caching
{
    public class NoClientCacheAttribute : IClientCacheAttribute
    {
        /// <summary>
        /// Default: false.
        /// </summary>
        public bool IncludeNonAjaxRequests { get; set; }

        public NoClientCacheAttribute()
            : this(false)
        {
            
        }

        public NoClientCacheAttribute(bool includeNonAjaxRequests)
        {
            IncludeNonAjaxRequests = includeNonAjaxRequests;
        }

        public virtual void Apply(ResultExecutingContext context)
        {
            if (IncludeNonAjaxRequests || context.HttpContext.Request.IsAjaxRequest())
            {
                context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, max-age=0";
                context.HttpContext.Response.Headers["Pragma"] = "no-cache";
                context.HttpContext.Response.Headers["Expires"] = "0";
            }
        }
    }
}
