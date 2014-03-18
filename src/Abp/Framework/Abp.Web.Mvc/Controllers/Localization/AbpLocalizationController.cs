using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Abp.Exceptions;
using Abp.Utils.Helpers;
using Abp.Web.Mvc.Models;
using Abp.WebApi.Controllers.Dynamic.Scripting.Localization;

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

        public ActionResult ChangeLanguage(string lang, string returnUrl = "")
        {
            if (!GlobalizationHelper.IsValidCultureCode(lang))
            {
                throw new AbpException("Unknown language: " + lang + ". It must be a valid culture!");
            }

            Response.Cookies.Add(new HttpCookie("Abp.Localization.Language", lang) { Expires = DateTime.Now.AddYears(2) });

            if (Request.IsAjaxRequest())
            {
                return Json(new AbpMvcAjaxResponse(), JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/");
        }
    }
}
