using System;
using System.Collections.Generic;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Auditing;
using Abp.Configuration;
using Abp.Localization;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using IUrlHelper = Abp.Web.Http.IUrlHelper;

namespace Abp.AspNetCore.Mvc.Controllers;

public class AbpLocalizationController : AbpController
{
    protected IUrlHelper UrlHelper;
    private readonly ISettingStore _settingStore;

    private readonly ITypedCache<string, Dictionary<string, SettingInfo>> _userSettingCache;

    public AbpLocalizationController(
        IUrlHelper urlHelper,
        ISettingStore settingStore,
        ICacheManager cacheManager)
    {
        UrlHelper = urlHelper;
        _settingStore = settingStore;
        _userSettingCache = cacheManager.GetUserSettingsCache();
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
            ChangeCultureForUser(cultureName);
        }

        if (Request.IsAjaxRequest())
        {
            return Json(new AjaxResponse());
        }

        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            var escapedReturnUrl = Uri.EscapeDataString(returnUrl);
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

        return LocalRedirect("/");
    }

    protected virtual void ChangeCultureForUser(string cultureName)
    {
        var languageSetting = _settingStore.GetSettingOrNull(
            AbpSession.TenantId,
            AbpSession.GetUserId(),
            LocalizationSettingNames.DefaultLanguage
        );

        if (languageSetting == null)
        {
            _settingStore.Create(new SettingInfo(
                AbpSession.TenantId,
                AbpSession.UserId,
                LocalizationSettingNames.DefaultLanguage,
                cultureName
            ));
        }
        else
        {
            _settingStore.Update(new SettingInfo(
                AbpSession.TenantId,
                AbpSession.UserId,
                LocalizationSettingNames.DefaultLanguage,
                cultureName
            ));
        }

        _userSettingCache.Remove(AbpSession.ToUserIdentifier().ToString());
    }
}