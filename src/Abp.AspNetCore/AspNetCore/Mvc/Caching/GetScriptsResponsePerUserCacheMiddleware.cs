using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore.Mvc.Caching
{
    public class GetScriptsResponsePerUserCacheMiddleware
    {
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
                        MaxAge = TimeSpan.FromMinutes(30),
                    };
            }

            await _next(context);
        }
    }
}