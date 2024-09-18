using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.HtmlSanitizer.Configuration;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.HtmlSanitizer;

public class HtmlSanitizerHelper : IHtmlSanitizerHelper
{
    private readonly IHtmlSanitizerConfiguration _configuration;
    private readonly IHtmlSanitizer _htmlSanitizer;

    public HtmlSanitizerHelper(IHtmlSanitizerConfiguration configuration, IHtmlSanitizer htmlSanitizer)
    {
        _configuration = configuration;
        _htmlSanitizer = htmlSanitizer;
    }

    public bool ShouldSanitizeContext(ActionExecutingContext context)
    {
        if (_configuration is null)
        {
            return false;
        }

        if (!_configuration.IsEnabledForGetRequests &&
            context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var methodInfo = context.ActionDescriptor.GetMethodInfo();

        if (methodInfo == null)
        {
            return false;
        }

        if (!methodInfo.IsPublic)
        {
            return false;
        }

        var classType = methodInfo.DeclaringType;

        if (methodInfo.IsDefined(typeof(DisableHtmlSanitizerAttribute), true))
        {
            return false;
        }

        if (classType != null)
        {
            if (classType.GetTypeInfo().IsDefined(typeof(DisableHtmlSanitizerAttribute), true))
            {
                return false;
            }

            if (classType.GetTypeInfo().IsDefined(typeof(HtmlSanitizerAttribute), true))
            {
                return true;
            }

            if (_configuration.Selectors.Any(selector => selector.Invoke(methodInfo)))
            {
                return true;
            }
        }

        return methodInfo.IsDefined(typeof(HtmlSanitizerAttribute), true);
    }

    public void SanitizeContext(ActionExecutingContext context)
    {
        foreach (var item in context.ActionDescriptor.Parameters)
        {
            if (!context.ActionArguments.ContainsKey(item.Name))
            {
                // This parameter was not provided in the request.
                continue;
            }

            var argumentItem = context.ActionArguments[item.Name];

            if (argumentItem is null)
            {
                continue;
            }

            if (argumentItem is string)
            {
                context.ActionArguments[item.Name] = SanitizeHtml(argumentItem.ToString());
                continue;
            }


            SanitizeObject(argumentItem);
        }
    }

    private void SanitizeObject(object item)
    {
        if (item is null)
        {
            return;
        }

        var classType = item.GetType();

        if (classType.GetTypeInfo().IsDefined(typeof(DisableHtmlSanitizerAttribute), true))
        {
            return;
        }

        switch (item)
        {
            case IDictionary dictionary:
                {
                    foreach (var value in dictionary.Values)
                    {
                        SanitizeObject(value);
                    }

                    break;
                }
            case IEnumerable enumerable:
                {
                    foreach (var listItem in enumerable)
                    {
                        SanitizeObject(listItem);
                    }

                    break;
                }
            case DateTime _:
                {
                    break;
                }
            default:
                {
                    var properties = classType.GetProperties();

                    foreach (var property in properties)
                    {
                        SanitizeProperty(item, property);
                    }

                    break;
                }
        }
    }

    private void SanitizeProperty(object item, PropertyInfo property)
    {
        var value = property.GetValue(item);

        if (value is string)
        {
            if (property.IsDefined(typeof(DisableHtmlSanitizerAttribute), true))
            {
                return;
            }

            var sanitizedText = SanitizeHtml(value.ToString());
            property.SetValue(item, sanitizedText);
            return;
        }

        if (value is null)
        {
            return;
        }

        SanitizeObject(property.GetValue(item));
    }

    private string SanitizeHtml(string html)
    {
        _htmlSanitizer.KeepChildNodes = _configuration.KeepChildNodes;

        return _htmlSanitizer.Sanitize(html);
    }
}