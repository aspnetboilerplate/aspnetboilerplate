using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using Abp.Web.Security;
using Abp.WebApi.Extensions;
using System.Net.Http;
using System.Linq;
using Abp.Extensions;

namespace Abp.WebApi.Security
{
    public static class CsrfTokenManagerWebApiExtensions
    {
        public static void SetCookie(this ICsrfTokenManager manager, HttpResponseHeaders headers)
        {
            headers.SetCookie(new Cookie(manager.Configuration.TokenCookieName, manager.TokenGenerator.Generate()));
        }

        public static bool IsValid(this ICsrfTokenManager manager, HttpRequestHeaders headers)
        {
            var csrfCookie = headers.GetCookies(manager.Configuration.TokenCookieName).LastOrDefault();
            if (csrfCookie == null)
            {
                return true;
            }

            var cookieTokenValue = csrfCookie[manager.Configuration.TokenCookieName].Value;
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
