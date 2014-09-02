using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Abp.Localization;
using Abp.Web.Localization;
using Abp.Web.Mvc.Models;

namespace Abp.Web.Mvc.Controllers.Localization
{
    public class AbpLocalizationController : AbpController
    {
        private readonly ILocalizationScriptManager _localizationScriptManager;

        public AbpLocalizationController(ILocalizationScriptManager localizationScriptManager)
        {
            _localizationScriptManager = localizationScriptManager;
        }

        public ContentResult GetScripts()
        {
            var script = _localizationScriptManager.GetLocalizationScript();
            return Content(script, "application/x-javascript", Encoding.UTF8);
        }

        public ActionResult ChangeCulture(string cultureName, string returnUrl = "")
        {
            if (!GlobalizationHelper.IsValidCultureCode(cultureName))
            {
                throw new AbpException("Unknown language: " + cultureName + ". It must be a valid culture!");
            }

            Response.Cookies.Add(new HttpCookie("Abp.Localization.CultureName", cultureName) { Expires = DateTime.Now.AddYears(2) });

            if (Request.IsAjaxRequest())
            {
                return Json(new MvcAjaxResponse(), JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/");
        }
    }
}
