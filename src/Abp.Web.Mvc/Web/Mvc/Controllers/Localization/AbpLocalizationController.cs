using System;
using System.Web;
using System.Web.Mvc;
using Abp.Auditing;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.Web.Configuration;
using Abp.Web.Http;
using Abp.Web.Models;

namespace Abp.Web.Mvc.Controllers.Localization
{
    public class AbpLocalizationController : AbpController
    {
        protected readonly IAbpWebLocalizationConfiguration WebLocalizationConfiguration;
        protected IUrlHelper UrlHelper;

        public AbpLocalizationController(IAbpWebLocalizationConfiguration webLocalizationConfiguration, IUrlHelper urlHelper)
        {
            WebLocalizationConfiguration = webLocalizationConfiguration;
            UrlHelper = urlHelper;
        }

        [DisableAuditing]
        public virtual ActionResult ChangeCulture(string cultureName, string returnUrl = "")
        {
            if (!GlobalizationHelper.IsValidCultureCode(cultureName))
            {
                throw new AbpException("Unknown language: " + cultureName + ". It must be a valid culture!");
            }

            Response.Cookies.Add(
                new HttpCookie(WebLocalizationConfiguration.CookieName, cultureName)
                {
                    Expires = Clock.Now.AddYears(2),
                    Path = Request.ApplicationPath
                }
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
                return Json(new AjaxResponse(), JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                var escapedReturnUrl = Uri.EscapeUriString(returnUrl);
                var localPath = UrlHelper.LocalPathAndQuery(escapedReturnUrl, Request.Url.Host, Request.Url.Port);
                if (!string.IsNullOrWhiteSpace(localPath))
                {
                    var unescapedLocalPath = Uri.UnescapeDataString(localPath);
                    if (Url.IsLocalUrl(unescapedLocalPath))
                    {
                        return Redirect(unescapedLocalPath);
                    }
                }
            }

            return Redirect(Request.ApplicationPath);
        }
    }
}
