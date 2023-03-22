using System;
using System.Reflection;
using System.Runtime.Serialization;
using Abp.Dependency;

namespace Abp.HtmlSanitizer;

public class HtmlSanitizerBuilder
{
    protected IHtmlSanitizerConfiguration HtmlSanitizerConfiguration { get; }

    public HtmlSanitizerBuilder()
    {
        HtmlSanitizerConfiguration = IocManager.Instance.Resolve<IHtmlSanitizerConfiguration>();
    }

    public HtmlSanitizerBuilder AddSelector<T>()
    {
        var updatedSelector = new Func<MethodInfo, bool>(methodInfo =>
        {
            var type = methodInfo.DeclaringType;
            
            return typeof(T).IsAssignableFrom(type) && type.IsClass;
        });

        HtmlSanitizerConfiguration.Selectors.Add(updatedSelector);
        return this;
    }
    
    public HtmlSanitizerBuilder AddSelector<T>(Func<T, string> selector)
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
                
            if (typeof(T).GetProperty(param) is {} propertyInfo)
            {
                return param.Equals(propertyInfo.Name);
            }

            return false;
        });

        HtmlSanitizerConfiguration.Selectors.Add(updatedSelector);
        return this;
    }
    
    public HtmlSanitizerBuilder EnableForGetRequests(bool enableForGetRequests = true)
    {
        HtmlSanitizerConfiguration.IsEnabledForGetRequests = enableForGetRequests;
        return this;
    }
    
    public HtmlSanitizerBuilder KeepChildNodes(bool keepChildNodes = true)
    {
        HtmlSanitizerConfiguration.KeepChildNodes = keepChildNodes;
        return this;
    }
}

