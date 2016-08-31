using System.Web;
using System.Web.Mvc;
using Abp.Auditing;
using Abp.Configuration;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.Web.Models;

namespace Abp.Web.Mvc.Controllers.Localization
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

            Response.Cookies.Add(new HttpCookie("Abp.Localization.CultureName", cultureName) { Expires = Clock.Now.AddYears(2) });

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
                return Json(new AjaxResponse(), JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect(Request.ApplicationPath);
        }
    }
}
