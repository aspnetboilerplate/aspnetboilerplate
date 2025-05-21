using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.HtmlSanitizer.Configuration;
using Ganss.Xss;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;

namespace Abp.HtmlSanitizer.Middleware;

public class MiddlewareHtmlSanitizerHelper(
    IHtmlSanitizerConfiguration configuration,
    IHtmlSanitizer htmlSanitizer)
    : HtmlSanitizerBase(configuration, htmlSanitizer), IMiddlewareHtmlSanitizerHelper
{
    private readonly IHtmlSanitizerConfiguration _configuration = configuration;

    public bool ShouldSanitizeContext(HttpContext context)
    {
        if (_configuration is null)
        {
            return false;
        }

        if (!_configuration.IsEnabledForGetRequests &&
            context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var endpoint = context.GetEndpoint();
        var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

        var methodInfo = actionDescriptor?.GetMethodInfo();

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

    public async Task SanitizeContext(HttpContext context)
    {
        context.Request.EnableBuffering();

        if (!context.Request.Body.CanRead) return;

        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true);
        var bodyStr = await reader.ReadToEndAsync();

        context.Request.Body.Position = 0;

        var sanitizedContent = bodyStr;

        if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
        {
            sanitizedContent = SanitizeJsonBody(context, bodyStr);
        }

        if (context.Request.ContentType != null &&
            context.Request.ContentType.Contains("application/x-www-form-urlencoded"))
        {
            sanitizedContent = SanitizeFormUrlEncodedBody(bodyStr);
        }

        var newBody = new MemoryStream();
        var writer = new StreamWriter(newBody);

        await writer.WriteAsync(sanitizedContent);
        await writer.FlushAsync();

        newBody.Position = 0;
        context.Request.Body = newBody;
    }

    private string SanitizeFormUrlEncodedBody(string bodyStr)
    {
        var decodedBodyStr = HttpUtility.UrlDecode(bodyStr);

        var properties = decodedBodyStr.Split('&');

        foreach (var property in properties)
        {
            var keyValuePair = property.Split('=');
            if (keyValuePair.Length != 2)
            {
                continue;
            }

            var value = keyValuePair[1];

            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            var sanitizedValue = SanitizeHtml(value);
            decodedBodyStr = decodedBodyStr.Replace(value, sanitizedValue);
        }

        return decodedBodyStr;
    }

    private string SanitizeJsonBody(HttpContext context, string bodyStr)
    {
        var inputParameterType = GetActionMethodInputParameterType(context);
        if (inputParameterType != null)
        {
            var inputObject = JsonConvert.DeserializeObject(bodyStr, inputParameterType);

            SanitizeObject(inputObject);
            return JsonConvert.SerializeObject(inputObject);
        }

        var json = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(bodyStr));

        var sanitizedContent = SanitizeHtml(json);
        return sanitizedContent;
    }

    private Type GetActionMethodInputParameterType(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        var methodInfo = actionDescriptor?.MethodInfo;
        var parameters = methodInfo?.GetParameters();

        if (parameters == null || !parameters.Any())
        {
            return null;
        }
            
        var parameter = parameters.FirstOrDefault();

        if (parameter == null)
        {
            return null;
        }
            
        return parameter.ParameterType;
    }
}