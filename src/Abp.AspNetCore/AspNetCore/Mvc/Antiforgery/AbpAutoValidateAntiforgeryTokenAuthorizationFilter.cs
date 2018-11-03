using Abp.Dependency;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Abp.AspNetCore.Mvc.Antiforgery
{
    public class AbpAutoValidateAntiforgeryTokenAuthorizationFilter : AutoValidateAntiforgeryTokenAuthorizationFilter, ITransientDependency
    {
        private readonly AntiforgeryOptions _antiforgeryOptions;
        private readonly CookieAuthenticationOptions _cookieAuthenticationOptions;

        public AbpAutoValidateAntiforgeryTokenAuthorizationFilter(
            IAntiforgery antiforgery, 
            ILoggerFactory loggerFactory,
            IOptions<AntiforgeryOptions> antiforgeryOptions,
            IOptions<CookieAuthenticationOptions> cookieAuthenticationOptions)
            : base(antiforgery, loggerFactory)
        {
            _antiforgeryOptions = antiforgeryOptions.Value;
            _cookieAuthenticationOptions = cookieAuthenticationOptions.Value;
        }

        protected override bool ShouldValidate(AuthorizationFilterContext context)
        {
            if (!base.ShouldValidate(context))
            {
                return false;
            }

            //Always perform antiforgery validation when request contains authentication cookie
            if (_cookieAuthenticationOptions.Cookie.Name != null &&
                context.HttpContext.Request.Cookies.ContainsKey(_cookieAuthenticationOptions.Cookie.Name))
            {
                return true;
            }

            //No need to validate if antiforgery cookie is not sent.
            //That means the request is sent from a non-browser client.
            //See https://github.com/aspnet/Antiforgery/issues/115
            if (!context.HttpContext.Request.Cookies.ContainsKey(_antiforgeryOptions.Cookie.Name))
            {
                return false;
            }

            // Anything else requires a token.
            return true;
        }
    }
}