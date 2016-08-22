using System.Web;
using System.Web.Helpers;
using Abp.Extensions;
using Abp.Web.Security.AntiForgery;

namespace Abp.Web.Security
{
    public static class AbpAntiForgeryTokenManagerMvcExtensions
    {
        public static void SetCookie(this IAbpAntiForgeryManager manager, HttpContextBase context)
        {
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