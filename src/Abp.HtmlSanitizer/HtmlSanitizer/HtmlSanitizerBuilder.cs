using System;
using System.Reflection;
using System.Runtime.Serialization;
using Abp.HtmlSanitizer.Configuration;

namespace Abp.HtmlSanitizer;

public static class HtmlSanitizerBuilder
{
    public static IHtmlSanitizerConfiguration AddSelector<T>(this IHtmlSanitizerConfiguration configuration)
    {
        var updatedSelector = new Func<MethodInfo, bool>(methodInfo =>
        {
            var type = methodInfo.DeclaringType;
            return typeof(T).IsAssignableFrom(type) && type.IsClass;
        });

        configuration.Selectors.Add(updatedSelector);
        return configuration;
    }

    public static IHtmlSanitizerConfiguration AddSelector<T>(this IHtmlSanitizerConfiguration configuration, Func<T, string> selector)
    {
        var updatedSelector = new Func<MethodInfo, bool>(methodInfo =>
        {
            var type = methodInfo.DeclaringType;

            if (!typeof(T).IsAssignableFrom(type) || !type.IsClass) return false;

            var instance = (T)FormatterServices.GetUninitializedObject(type);

            var param = selector(instance);

            if (param.Equals(methodInfo.Name))
            {
                return true;
            }

            if (typeof(T).GetProperty(param) is { } propertyInfo)
            {
                return param.Equals(propertyInfo.Name);
            }

            return false;
        });

        configuration.Selectors.Add(updatedSelector);
        return configuration;
    }

    public static IHtmlSanitizerConfiguration EnableForGetRequests(this IHtmlSanitizerConfiguration configuration, bool enableForGetRequests = true)
    {
        configuration.IsEnabledForGetRequests = enableForGetRequests;
        return configuration;
    }

    public static IHtmlSanitizerConfiguration KeepChildNodes(this IHtmlSanitizerConfiguration configuration, bool keepChildNodes = true)
    {
        configuration.KeepChildNodes = keepChildNodes;
        return configuration;
    }
}