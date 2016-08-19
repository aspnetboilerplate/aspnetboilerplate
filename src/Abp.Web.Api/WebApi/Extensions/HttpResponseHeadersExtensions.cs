using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Abp.WebApi.Extensions
{
    public static class HttpResponseHeadersExtensions
    {
        public static void SetCookie(this HttpResponseHeaders headers, Cookie cookie)
        {
            Check.NotNull(headers, nameof(headers));
            Check.NotNull(cookie, nameof(cookie));

            var cookieBuilder = new StringBuilder(HttpUtility.UrlEncode(cookie.Name) + "=" + HttpUtility.UrlEncode(cookie.Value));
            if (cookie.HttpOnly)
            {
                cookieBuilder.Append("; HttpOnly");
            }

            if (cookie.Secure)
            {
                cookieBuilder.Append("; Secure");
            }

            headers.Add("Set-Cookie", cookieBuilder.ToString());
        }
    }
}