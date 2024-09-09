using System.Threading.Tasks;
using Abp.Dependency;
using Microsoft.AspNetCore.Http;

namespace Abp.HtmlSanitizer.Middleware
{
    public interface IMiddlewareHtmlSanitizerHelper : ISingletonDependency
    {
        bool ShouldSanitizeContext(HttpContext context);
        
        Task SanitizeContext(HttpContext context);
    }
}