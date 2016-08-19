using System.Net;
using System.Net.Http.Headers;
using Abp.Web.Security;
using Abp.WebApi.Extensions;

namespace Abp.WebApi.Security
{
    public static class CsrfTokenManagerWebApiExtensions
    {
        public static void SetCookie(this ICsrfTokenManager manager, HttpResponseHeaders headers)
        {
            headers.SetCookie(new Cookie(manager.Configuration.TokenCookieName, manager.TokenGenerator.Generate()));
        }
    }
}
