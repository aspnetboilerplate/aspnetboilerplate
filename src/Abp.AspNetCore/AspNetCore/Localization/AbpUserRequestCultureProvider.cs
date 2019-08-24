using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using JetBrains.Annotations;

namespace Abp.AspNetCore.Localization
{
    public class AbpUserRequestCultureProvider : RequestCultureProvider
    {
        public CookieRequestCultureProvider CookieProvider { get; set; }
        public AbpLocalizationHeaderRequestCultureProvider HeaderProvider { get; set; }
        public ILogger Logger { get; set; }

        public AbpUserRequestCultureProvider()
        {
            Logger = NullLogger.Instance;
        }

        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var abpSession = httpContext.RequestServices.GetRequiredService<IAbpSession>();
            if (abpSession.UserId == null)
            {
                return null;
            }

            var settingManager = httpContext.RequestServices.GetRequiredService<ISettingManager>();

            var userCulture = await settingManager.GetSettingValueForUserAsync(
                LocalizationSettingNames.DefaultLanguage,
                abpSession.TenantId,
                abpSession.UserId.Value,
                fallbackToDefault: false
            );

            if (!userCulture.IsNullOrEmpty())
            {
                Logger.DebugFormat("{0} - Read from user settings", nameof(AbpUserRequestCultureProvider));
                Logger.DebugFormat("Using Culture:{0} , UICulture:{1}", userCulture, userCulture);
                return new ProviderCultureResult(userCulture, userCulture);
            }

            ProviderCultureResult result = null;
            string cultureName = null;
            var cookieResult = await GetResultOrNull(httpContext, CookieProvider);
            if (cookieResult != null && cookieResult.Cultures.Any())
            {
                var cookieCulture = cookieResult.Cultures.First().Value;
                var cookieUICulture = cookieResult.UICultures.First().Value;

                Logger.DebugFormat("{0} - Read from cookie", nameof(AbpUserRequestCultureProvider));
                Logger.DebugFormat("Using Culture:{0} , UICulture:{1}", cookieCulture, cookieUICulture);

                result = cookieResult;
                cultureName = cookieCulture ?? cookieUICulture;
            }

            if (result == null || !result.Cultures.Any())
            {
                var headerResult = await GetResultOrNull(httpContext, HeaderProvider);
                if (headerResult != null && headerResult.Cultures.Any())
                {
                    var headerCulture = headerResult.Cultures.First().Value;
                    var headerUICulture = headerResult.UICultures.First().Value;

                    Logger.DebugFormat("{0} - Read from header", nameof(AbpUserRequestCultureProvider));
                    Logger.DebugFormat("Using Culture:{0} , UICulture:{1}", headerCulture, headerUICulture);

                    result = headerResult;
                    cultureName = headerCulture ?? headerUICulture;
                }
            }

            if (!cultureName.IsNullOrEmpty())
            {
                //Try to set user's language setting from cookie/header if available.
                await settingManager.ChangeSettingForUserAsync(
                    abpSession.ToUserIdentifier(),
                    LocalizationSettingNames.DefaultLanguage,
                    cultureName
                );
            }

            return result;
        }

        protected virtual async Task<ProviderCultureResult> GetResultOrNull([NotNull] HttpContext httpContext, [CanBeNull] IRequestCultureProvider provider)
        {
            if (provider == null)
            {
                return null;
            }

            return await provider.DetermineProviderCultureResult(httpContext);
        }
    }
}
