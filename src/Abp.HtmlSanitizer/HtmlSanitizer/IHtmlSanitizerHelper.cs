using System.Threading.Tasks;
using Abp.Dependency;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.HtmlSanitizer
{
    public interface IHtmlSanitizerHelper : ISingletonDependency
    {
        bool ShouldSanitizeContext(ActionExecutingContext actionExecutingContext);
        
        bool ShouldSanitizeContext(HttpContext context);
    
        void SanitizeContext(ActionExecutingContext context);
        
        Task SanitizeContext(HttpContext context);
    }
}