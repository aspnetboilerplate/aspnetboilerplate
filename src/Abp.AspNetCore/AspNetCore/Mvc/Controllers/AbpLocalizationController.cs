using System;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Auditing;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using IUrlHelper = Abp.Web.Http.IUrlHelper;

namespace Abp.AspNetCore.Mvc.Controllers
{
    public class AbpLocalizationController : AbpController
    {
        protected IUrlHelper UrlHelper;

        public AbpLocalizationController(IUrlHelper urlHelper)
        {
            UrlHelper = urlHelper;
        }

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
                new CookieOptions
                {
                    Expires = Clock.Now.AddYears(2),
                    HttpOnly = true 
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
                return Json(new AjaxResponse());
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                var escapedReturnUrl = Uri.EscapeUriString(returnUrl);
                var localPath = UrlHelper.LocalPathAndQuery(escapedReturnUrl, Request.Host.Host, Request.Host.Port);
                if (!string.IsNullOrWhiteSpace(localPath))
                {
                    var unescapedLocalPath = Uri.UnescapeDataString(localPath);
                    if (Url.IsLocalUrl(unescapedLocalPath))
                    {
                        return LocalRedirect(unescapedLocalPath);
                    }
                }
            }

            return LocalRedirect("/"); //TODO: Go to app root
        }
    }
}
