using System;
using Abp.Dependency;
using Abp.Web.Security.AntiForgery;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Abp.AspNetCore.Mvc.Antiforgery
{
    public class AbpAutoValidateAntiforgeryTokenAuthorizationFilter : AbpValidateAntiforgeryTokenAuthorizationFilter, ITransientDependency
    {
        private readonly AntiforgeryOptions _antiforgeryOptions;
        private readonly IOptionsSnapshot<CookieAuthenticationOptions> _namedOptionsAccessor;
        private readonly IAbpAntiForgeryConfiguration _antiForgeryConfiguration;

        public AbpAutoValidateAntiforgeryTokenAuthorizationFilter(
            IAntiforgery antiforgery,
            IOptions<AntiforgeryOptions> antiforgeryOptions,
            IAbpAntiForgeryConfiguration antiForgeryConfiguration,
            IOptionsSnapshot<CookieAuthenticationOptions> namedOptionsAccessor,
            ILogger logger)
            : base(antiforgery, antiforgeryOptions, antiForgeryConfiguration, namedOptionsAccessor, logger)
        {
            _namedOptionsAccessor = namedOptionsAccessor;
            _antiForgeryConfiguration = antiForgeryConfiguration;
            _antiforgeryOptions = antiforgeryOptions.Value;
        }

        protected override bool ShouldValidate(AuthorizationFilterContext context)
        {
            if (!ShouldValidateInternal(context))
            {
                return false;
            }

            var cookieAuthenticationOptions = _namedOptionsAccessor.Get(_antiForgeryConfiguration.AuthorizationCookieName);

            //Always perform antiforgery validation when request contains authentication cookie
            if (cookieAuthenticationOptions?.Cookie.Name != null &&
                context.HttpContext.Request.Cookies.ContainsKey(cookieAuthenticationOptions.Cookie.Name))
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

        private static bool ShouldValidateInternal(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var method = context.HttpContext.Request.Method;
            if (string.Equals("GET", method, StringComparison.OrdinalIgnoreCase) ||
                string.Equals("HEAD", method, StringComparison.OrdinalIgnoreCase) ||
                string.Equals("TRACE", method, StringComparison.OrdinalIgnoreCase) ||
                string.Equals("OPTIONS", method, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Anything else requires a token.
            return true;
        }
    }
}