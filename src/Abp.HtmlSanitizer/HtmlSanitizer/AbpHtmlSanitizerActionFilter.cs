using System.Threading.Tasks;
using Abp.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.HtmlSanitizer;

public class AbpHtmlSanitizerActionFilter : IAsyncActionFilter, ISingletonDependency
{
    private readonly IHtmlSanitizerHelper _htmlSanitizerHelper;

    public AbpHtmlSanitizerActionFilter(IHtmlSanitizerHelper htmlSanitizerHelper)
    {
        _htmlSanitizerHelper = htmlSanitizerHelper;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!_htmlSanitizerHelper.ShouldSanitizeContext(context))
        {
            await next();
            return;
        }

        _htmlSanitizerHelper.SanitizeContext(context);
        await next();
    }
}