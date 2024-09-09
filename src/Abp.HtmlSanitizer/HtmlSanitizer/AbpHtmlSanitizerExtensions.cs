using Abp.HtmlSanitizer.ActionFilter;
using Abp.HtmlSanitizer.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Abp.HtmlSanitizer
{
    public static class AbpHtmlSanitizerExtensions
    {
        public static void AddAbpHtmlSanitizer(this MvcOptions options)
        {
            options.Filters.AddService(typeof(AbpHtmlSanitizerActionFilter));
        }
        
        public static IApplicationBuilder UseAbpHtmlSanitizer(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AbpHtmlSanitizerMiddleware>();
        }
    }
}