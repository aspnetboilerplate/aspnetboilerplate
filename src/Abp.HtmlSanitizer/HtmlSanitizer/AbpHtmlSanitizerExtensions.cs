using Microsoft.AspNetCore.Mvc;

namespace Abp.HtmlSanitizer.HtmlSanitizer;

public static class AbpHtmlSanitizerExtensions
{
    public static void AddAbpHtmlSanitizer(this MvcOptions options)
    {
        options.Filters.AddService(typeof(AbpHtmlSanitizerActionFilter));
    }
}