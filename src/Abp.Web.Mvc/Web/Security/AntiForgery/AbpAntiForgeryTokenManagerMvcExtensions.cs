using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using Abp.Extensions;

namespace Abp.Web.Security.AntiForgery
{
    public static class AbpAntiForgeryTokenManagerMvcExtensions
    {
        public static void SetCookie(this IAbpAntiForgeryManager manager, HttpContextBase context, IIdentity identity = null)
        {
            if (identity != null)
            {
                context.User = new ClaimsPrincipal(identity);
            }

            context.Response.Cookies.Add(new HttpCookie(manager.Configuration.TokenCookieName, manager.GenerateToken()));
        }

        public static bool IsValid(this IAbpAntiForgeryManager manager, HttpContextBase context)
        {
            var cookieValue = GetCookieValue(context);
            if (cookieValue.IsNullOrEmpty())
            {
                return true;
            }

            var formOrHeaderValue = manager.Configuration.GetFormOrHeaderValue(context);
            if (formOrHeaderValue.IsNullOrEmpty())
            {
                return false;
            }

            return manager.IsValid(cookieValue, formOrHeaderValue);
        }

        private static string GetCookieValue(HttpContextBase context)
        {
            var cookie = context.Request.Cookies[AntiForgeryConfig.CookieName];
            return cookie?.Value;
        }

        private static string GetFormOrHeaderValue(this IAbpAntiForgeryConfiguration configuration, HttpContextBase context)
        {
            var formValue = context.Request.Form["__RequestVerificationToken"];
            if (!formValue.IsNullOrEmpty())
            {
                return formValue;
            }

            return context.Request.Headers[configuration.TokenHeaderName];
        }
    }
}