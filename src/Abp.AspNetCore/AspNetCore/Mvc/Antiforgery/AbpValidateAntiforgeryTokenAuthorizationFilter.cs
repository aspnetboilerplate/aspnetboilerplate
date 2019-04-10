using Abp.Web.Security.AntiForgery;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Abp.AspNetCore.Mvc.Antiforgery
{
    public class AbpValidateAntiforgeryTokenAuthorizationFilter : ValidateAntiforgeryTokenAuthorizationFilter
    {
        private readonly AntiforgeryOptions _antiforgeryOptions;
        private readonly IOptionsSnapshot<CookieAuthenticationOptions> _namedOptionsAccessor;
        private readonly IAbpAntiForgeryConfiguration _antiForgeryConfiguration;

        public AbpValidateAntiforgeryTokenAuthorizationFilter(
            IAntiforgery antiforgery,
            ILoggerFactory loggerFactory,
            IOptions<AntiforgeryOptions> antiforgeryOptions,
            IAbpAntiForgeryConfiguration antiForgeryConfiguration,
            IOptionsSnapshot<CookieAuthenticationOptions> namedOptionsAccessor)
            : base(antiforgery, loggerFactory)
        {
            _antiforgeryOptions = antiforgeryOptions.Value;
            _namedOptionsAccessor = namedOptionsAccessor;
            _antiForgeryConfiguration = antiForgeryConfiguration;
        }

        protected override bool ShouldValidate(AuthorizationFilterContext context)
        {
            if (!base.ShouldValidate(context))
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
    }
}
