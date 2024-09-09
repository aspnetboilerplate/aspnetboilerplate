using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Abp.HtmlSanitizer.Middleware
{
    public class AbpHtmlSanitizerMiddleware(
        RequestDelegate next,
        IMiddlewareHtmlSanitizerHelper htmlSanitizerHelper)
    {
        public async Task Invoke(HttpContext httpContext)
        {
            if (!htmlSanitizerHelper.ShouldSanitizeContext(httpContext))
            {
                await next(httpContext);
                return;
            }
            
            await htmlSanitizerHelper.SanitizeContext(httpContext);
            await next(httpContext);
        }
    }
}
