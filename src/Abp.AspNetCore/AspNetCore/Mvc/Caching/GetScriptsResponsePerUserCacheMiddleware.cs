using System;
using System.Threading.Tasks;
using Abp.Configuration;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore.Mvc.Caching
{
    public class GetScriptsResponsePerUserCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IGetScriptsResponsePerUserConfiguration _configuration;

        public GetScriptsResponsePerUserCacheMiddleware(RequestDelegate next,
            IGetScriptsResponsePerUserConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            if (_configuration.IsEnabled && context.Request.Path == "/AbpScripts/GetScripts")
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = _configuration.MaxAge,
                    };
            }

            await _next(context);
        }
    }
}