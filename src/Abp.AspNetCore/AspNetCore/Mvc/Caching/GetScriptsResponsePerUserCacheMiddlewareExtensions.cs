using System;
using Microsoft.AspNetCore.Builder;

namespace Abp.AspNetCore.Mvc.Caching
{
    public static class GetScriptsResponsePerUserCacheMiddlewareExtensions
    {
        /// <summary>
        /// Implements GetScriptsResponsePerUserCacheMiddleware middleware with given maxAge
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="maxAge">Default is 30min</param>
        public static IApplicationBuilder UseGetScriptsResponsePerUserCache(
            this IApplicationBuilder builder, TimeSpan? maxAge = null)
        {
            if (maxAge != null)
            {
                GetScriptsResponsePerUserCacheMiddleware.MaxAge = maxAge;
            }

            return builder.UseMiddleware<GetScriptsResponsePerUserCacheMiddleware>();
        }
    }
}