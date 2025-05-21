using System;
using System.Linq;
using System.Reflection;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.HtmlSanitizer.Configuration;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.HtmlSanitizer.ActionFilter
{
    public class ActionFilterHtmlSanitizerHelper(
        IHtmlSanitizerConfiguration configuration,
        IHtmlSanitizer htmlSanitizer)
        : HtmlSanitizerBase(configuration, htmlSanitizer), IActionFilterHtmlSanitizerHelper
    {
        private readonly IHtmlSanitizerConfiguration _configuration = configuration;

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
    }
}