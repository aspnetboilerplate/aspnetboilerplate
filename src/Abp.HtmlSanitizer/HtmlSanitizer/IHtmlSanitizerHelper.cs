using Abp.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.HtmlSanitizer;

public interface IHtmlSanitizerHelper : ISingletonDependency
{
    bool ShouldSanitizeContext(ActionExecutingContext actionExecutingContext);

    void SanitizeContext(ActionExecutingContext context);
}