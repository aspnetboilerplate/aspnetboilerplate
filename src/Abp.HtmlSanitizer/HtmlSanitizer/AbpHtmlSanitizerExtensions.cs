using System;
using Abp.AspNetCore.Uow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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