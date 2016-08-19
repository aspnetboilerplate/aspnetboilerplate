using System.Web;

namespace Abp.Web.Security
{
    public static class CsrfTokenManagerMvcExtensions
    {
        public static void SetCookie(this ICsrfTokenManager manager,HttpContextBase context)
        {
            context.Response.Cookies.Add(new HttpCookie(manager.Configuration.TokenCookieName, manager.TokenGenerator.Generate()));
        }
    }
}