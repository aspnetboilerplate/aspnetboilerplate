using System.Reflection;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Dependency;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.HtmlSanitizer.HtmlSanitizer.Interceptor;

public class AbpHtmlSanitizerActionFilter : IAsyncActionFilter, ITransientDependency
{
    private readonly IHtmlSanitizer _htmlSanitizer;

    public AbpHtmlSanitizerActionFilter(IHtmlSanitizer htmlSanitizer)
    {
        _htmlSanitizer = htmlSanitizer;
     }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (GetSanitizeHtmlAttributeOrNull(context) == null || !context.ActionDescriptor.IsControllerAction())
        {
            await next();
            return;
        }
        
        var stringArgs = context.ActionArguments.Where(pair => pair.Value is string).ToList();

        foreach (var keyValue in stringArgs)
        {
            var safeValue = _htmlSanitizer.Sanitize(keyValue.Value!.ToString()!);
            context.ActionArguments[keyValue.Key] = safeValue;
        }
        
        await next();
    }
    
    private SanitizeHtmlAttribute? GetSanitizeHtmlAttributeOrNull(ActionExecutingContext context)
    {
        return context.ActionDescriptor.GetMethodInfo().GetCustomAttribute<SanitizeHtmlAttribute>();
    }
}
