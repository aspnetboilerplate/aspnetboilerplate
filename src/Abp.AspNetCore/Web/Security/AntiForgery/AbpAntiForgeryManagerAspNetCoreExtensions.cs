using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;

namespace Abp.Web.Security.AntiForgery
{
    public static class AbpAntiForgeryManagerAspNetCoreExtensions
    {
        public static void SetCookie(this IAbpAntiForgeryManager manager, HttpContext context, IIdentity identity = null)
        {
            if (identity != null)
            {
                context.User = new ClaimsPrincipal(identity);
            }

            context.Response.Cookies.Append(manager.Configuration.TokenCookieName, manager.GenerateToken());
        }
    }
}
