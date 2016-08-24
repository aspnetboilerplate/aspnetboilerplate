using Abp.AspNetCore.Mvc.Extensions;
using Abp.Auditing;
using Abp.Configuration;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.Mvc.Controllers
{
    public class AbpLocalizationController : AbpController
    {
        [DisableAuditing]
        public virtual ActionResult ChangeCulture(string cultureName, string returnUrl = "")
        {
            if (!GlobalizationHelper.IsValidCultureCode(cultureName))
            {
                throw new AbpException("Unknown language: " + cultureName + ". It must be a valid culture!");
            }

            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureName, cultureName));

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                cookieValue,
                new CookieOptions {Expires = Clock.Now.AddYears(2)}
            );

            if (AbpSession.UserId.HasValue)
            {
                SettingManager.ChangeSettingForUser(
                    AbpSession.ToUserIdentifier(),
                    LocalizationSettingNames.DefaultLanguage,
                    cultureName
                );
            }

            if (Request.IsAjaxRequest())
            {
                return Json(new AjaxResponse());
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/"); //TODO: Go to app root
        }
    }
}
