using Abp.Dependency;
using Abp.HtmlSanitizer.HtmlSanitizer.Interceptor;
using Castle.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.HtmlSanitizer.HtmlSanitizer;

public static class HtmlSanitizerExtensions
{
    public static void AddAbpHtmlSanitizer(this MvcOptions options)
    {
        options.Filters.AddService(typeof(AbpHtmlSanitizerActionFilter));
    }
}