using Abp.HtmlSanitizer.HtmlSanitizer.Interceptor;
using Microsoft.AspNetCore.Mvc;

namespace Abp.HtmlSanitizer.HtmlSanitizer;

public static class HtmlSanitizerExtensions
{
    public static void AddAbpHtmlSanitizer(this MvcOptions options)
    {
        options.Filters.AddService(typeof(AbpHtmlSanitizerActionFilter));
    }
}