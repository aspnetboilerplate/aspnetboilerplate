using System.Reflection;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Dependency;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.HtmlSanitizer.HtmlSanitizer;

public class AbpHtmlSanitizerActionFilter : IAsyncActionFilter, ITransientDependency
{
    private readonly IHtmlSanitizer _htmlSanitizer;

    public AbpHtmlSanitizerActionFilter(IHtmlSanitizer htmlSanitizer)
    {
        _htmlSanitizer = htmlSanitizer;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var sanitizerAttr = GetSanitizeHtmlAttributeOrNull(context);

        if (sanitizerAttr is {IsDisabled: true} || !context.ActionDescriptor.IsControllerAction())
        {
            await next();
            return;
        }

        SanitizeActionArguments(context, sanitizerAttr);

        await next();
    }

    private SanitizeHtmlAttribute? GetSanitizeHtmlAttributeOrNull(ActionExecutingContext context)
    {
        return context.ActionDescriptor.GetMethodInfo().GetCustomAttribute<SanitizeHtmlAttribute>();
    }

    private void SanitizeActionArguments(ActionExecutingContext context, SanitizeHtmlAttribute? attr)
    {
        foreach (var item in context.ActionDescriptor.Parameters)
        {
            if (item.ParameterType.GetCustomAttribute(typeof(SanitizeHtmlAttribute), true) is SanitizeHtmlAttribute propertyAttr)
            {
                attr = propertyAttr;

                if (attr.IsDisabled)
                {
                    continue;
                }
            }

            var argumentItem = context.ActionArguments[item.Name];
            
            if (argumentItem is string)
            {
                if (attr is null)
                {
                    continue;
                }
                
                context.ActionArguments[item.Name] = SanitizeHtml(argumentItem.ToString()!, attr.KeepChildNodes);
                continue;
            }
            
            SanitizeObject(context.ActionArguments[item.Name], attr);
        }
    }

    private void SanitizeObject(object? item, SanitizeHtmlAttribute? attr)
    {
        var properties = item?.GetType().GetProperties();

        if (properties == null) return;

        foreach (var property in properties)
        {
            SanitizeProperty(item, attr, property);
        }
    }

    private void SanitizeProperty(object? item, SanitizeHtmlAttribute? attr, PropertyInfo property)
    {
        if (GetPropertyAttributeOrNull(attr, property) is not { } temp)
        {
            return;
        }

        var value = property.GetValue(item);

        if (value is string)
        {
            var sanitizedText = SanitizeHtml(value.ToString()!, temp.KeepChildNodes);
            property.SetValue(item, sanitizedText);
            return;
        }

        SanitizeObject(property.GetValue(item), temp);
    }

    private static SanitizeHtmlAttribute? GetPropertyAttributeOrNull(SanitizeHtmlAttribute? attr, PropertyInfo property)
    {
        var temp = attr;

        if (property.GetCustomAttribute(typeof(SanitizeHtmlAttribute), true) is not SanitizeHtmlAttribute propertyAttr)
        {
            return temp;
        }
        
        temp = propertyAttr;

        return temp.IsDisabled ? null : temp;
    }

    private string SanitizeHtml(string html, bool keepChildNodes)
    {
        _htmlSanitizer.KeepChildNodes = keepChildNodes;
        return _htmlSanitizer.Sanitize(html);
    }
}