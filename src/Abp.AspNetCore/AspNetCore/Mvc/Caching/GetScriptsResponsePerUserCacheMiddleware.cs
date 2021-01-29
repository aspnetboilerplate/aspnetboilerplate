using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore.Mvc.Caching
{
    public class GetScriptsResponsePerUserCacheMiddleware
    {
        internal static TimeSpan? MaxAge = TimeSpan.FromMinutes(30);
        private readonly RequestDelegate _next;

        public GetScriptsResponsePerUserCacheMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/AbpScripts/GetScripts")
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = MaxAge,
                    };
            }

            await _next(context);
        }
    }
}