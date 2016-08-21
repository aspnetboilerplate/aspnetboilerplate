using System.Web;
using Abp.Web.Security.AntiForgery;

namespace Abp.Web.Security
{
    public static class AbpAntiForgeryTokenManagerMvcExtensions
    {
        public static void SetCookie(this IAbpAntiForgeryTokenManager manager,HttpContextBase context)
        {
            context.Response.Cookies.Add(new HttpCookie(manager.Configuration.TokenCookieName, manager.TokenGenerator.Generate()));
        }
    }
}