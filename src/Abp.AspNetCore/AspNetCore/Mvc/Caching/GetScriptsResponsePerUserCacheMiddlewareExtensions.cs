using Microsoft.AspNetCore.Builder;

namespace Abp.AspNetCore.Mvc.Caching
{
    public static class GetScriptsResponsePerUserCacheMiddlewareExtensions
    {
        public static IApplicationBuilder UseGetScriptsResponsePerUserCache(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GetScriptsResponsePerUserCacheMiddleware>();
        }
    }
}