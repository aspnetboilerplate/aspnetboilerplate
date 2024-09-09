using System.Threading.Tasks;
using Abp.AspNetCore.Uow;
using Abp.Domain.Uow;
using Abp.HtmlSanitizer.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Abp.HtmlSanitizer
{
    public class AbpHtmlSanitizerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IHtmlSanitizerHelper _htmlSanitizerHelper;

        public AbpHtmlSanitizerMiddleware(
            RequestDelegate next, 
            IUnitOfWorkManager unitOfWorkManager, 
            IHtmlSanitizerHelper htmlSanitizerHelper)
        {
            _next = next;
            _unitOfWorkManager = unitOfWorkManager;
            _htmlSanitizerHelper = htmlSanitizerHelper;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!_htmlSanitizerHelper.ShouldSanitizeContext(httpContext))
            {
                await _next(httpContext);
                return;
            }
            
            await _htmlSanitizerHelper.SanitizeContext(httpContext);
            await _next(httpContext);
        }
    }
}
