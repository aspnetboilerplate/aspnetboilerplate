using Abp.Auditing;
using Abp.Localization;
using Abp.Timing;
using Microsoft.AspNetCore.Http;
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

            Response.Cookies.Append("Abp.Localization.CultureName", cultureName, new CookieOptions {Expires = Clock.Now.AddYears(2) });

            //if (Request.IsAjaxRequest())
            //{
            //    return Json(new MvcAjaxResponse(), JsonRequestBehavior.AllowGet);
            //}

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/");
            //return Redirect(Request.ApplicationPath);
        }
    }
}
