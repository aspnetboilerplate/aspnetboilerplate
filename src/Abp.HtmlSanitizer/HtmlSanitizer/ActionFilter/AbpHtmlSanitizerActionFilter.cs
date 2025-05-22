using System.Threading.Tasks;
using Abp.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.HtmlSanitizer.ActionFilter;

public class AbpHtmlSanitizerActionFilter(IActionFilterHtmlSanitizerHelper htmlSanitizerHelper)
    : IAsyncActionFilter, ISingletonDependency
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!htmlSanitizerHelper.ShouldSanitizeContext(context))
        {
            await next();
            return;
        }

        htmlSanitizerHelper.SanitizeContext(context);
        await next();
    }
}