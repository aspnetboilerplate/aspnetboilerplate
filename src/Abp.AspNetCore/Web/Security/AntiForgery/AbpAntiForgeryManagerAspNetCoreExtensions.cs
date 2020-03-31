using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;

namespace Abp.Web.Security.AntiForgery
{
    public static class AbpAntiForgeryManagerAspNetCoreExtensions
    {
        public static void SetCookie(this IAbpAntiForgeryManager manager, HttpContext context, IIdentity identity = null, CookieOptions cookieOptions = null)
        {
            if (identity != null)
            {
                context.User = new ClaimsPrincipal(identity);
            }

            if (cookieOptions != null)
            {
                context.Response.Cookies.Append(manager.Configuration.TokenCookieName, manager.GenerateToken(), cookieOptions);
            }
            else
            {
                context.Response.Cookies.Append(manager.Configuration.TokenCookieName, manager.GenerateToken());
            }
        }
    }
}
