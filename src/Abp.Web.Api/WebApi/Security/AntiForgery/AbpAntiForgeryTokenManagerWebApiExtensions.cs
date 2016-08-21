using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Abp.Extensions;
using Abp.Web.Security.AntiForgery;
using Abp.WebApi.Extensions;

namespace Abp.WebApi.Security.AntiForgery
{
    public static class AbpAntiForgeryTokenManagerWebApiExtensions
    {
        public static void SetCookie(this IAbpAntiForgeryTokenManager manager, HttpResponseHeaders headers)
        {
            headers.SetCookie(new Cookie(manager.Configuration.TokenCookieName, manager.TokenGenerator.Generate()));
        }

        public static bool IsValid(this IAbpAntiForgeryTokenManager manager, HttpRequestHeaders headers)
        {
            var antiForgeryCookie = headers.GetCookies(manager.Configuration.TokenCookieName).LastOrDefault();
            if (antiForgeryCookie == null)
            {
                return true;
            }

            var cookieTokenValue = antiForgeryCookie[manager.Configuration.TokenCookieName].Value;
            if (cookieTokenValue.IsNullOrEmpty())
            {
                return true;
            }

            IEnumerable<string> tokenHeaderValues;
            if (!headers.TryGetValues(manager.Configuration.TokenHeaderName, out tokenHeaderValues))
            {
                return false;
            }

            var headerTokenValue = tokenHeaderValues.Last();
            if (headerTokenValue != cookieTokenValue)
            {
                return false;
            }

            return true;
        }
    }
}
