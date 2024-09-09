using System;
using System.Collections;
using System.Reflection;
using Abp.HtmlSanitizer.Configuration;
using Ganss.Xss;

namespace Abp.HtmlSanitizer;

public abstract class HtmlSanitizerBase(IHtmlSanitizerConfiguration configuration, IHtmlSanitizer htmlSanitizer)
{
    protected void SanitizeObject(object item)
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

    protected string SanitizeHtml(string html)
    {
        htmlSanitizer.KeepChildNodes = configuration.KeepChildNodes;

        return htmlSanitizer.Sanitize(html);
    }
}