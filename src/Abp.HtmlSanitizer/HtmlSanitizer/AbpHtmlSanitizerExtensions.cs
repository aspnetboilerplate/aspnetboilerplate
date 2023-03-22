using Microsoft.AspNetCore.Mvc;

namespace Abp.HtmlSanitizer;

public static class AbpHtmlSanitizerExtensions
{
    public static HtmlSanitizerBuilder AddAbpHtmlSanitizer(this MvcOptions options)
    {
        options.Filters.AddService(typeof(AbpHtmlSanitizerActionFilter));

        return new HtmlSanitizerBuilder();
    }
}