using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Abp.HtmlSanitizer.Configuration;
using Ganss.Xss;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Abp.HtmlSanitizer.Middleware
{
    public class MiddlewareHtmlSanitizerHelper(
        IHtmlSanitizerConfiguration configuration,
        IHtmlSanitizer htmlSanitizer)
        : HtmlSanitizerBase(configuration, htmlSanitizer), IMiddlewareHtmlSanitizerHelper
    {
        private readonly IHtmlSanitizerConfiguration _configuration = configuration;

        public bool ShouldSanitizeContext(HttpContext context)
        {
            if (_configuration == null)
            {
                return false;
            }
            
            if (!_configuration.IsEnabledForGetRequests &&
                context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
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
                sanitizedContent = SanitizeJsonBody(bodyStr);
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

        private string SanitizeJsonBody(string bodyStr)
        {
            var json = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(bodyStr));
            
            var sanitizedContent = SanitizeHtml(json);
            return sanitizedContent;
        }
    }
}