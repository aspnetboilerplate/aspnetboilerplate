using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Abp.Localization;
using Abp.Extensions;
using JetBrains.Annotations;

namespace Abp.AspNetCore.Localization
{
    public class AbpUserRequestCultureProvider : RequestCultureProvider
    {
        public CookieRequestCultureProvider CookieProvider { get; set; }
        public AbpLocalizationHeaderRequestCultureProvider HeaderProvider { get; set; }

        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var abpSession = httpContext.RequestServices.GetRequiredService<IAbpSession>();
            if (abpSession.UserId == null)
            {
                return null;
            }

            var settingManager = httpContext.RequestServices.GetRequiredService<ISettingManager>();

            var culture = await settingManager.GetSettingValueForUserAsync(
                LocalizationSettingNames.DefaultLanguage,
                abpSession.TenantId,
                abpSession.UserId.Value,
                fallbackToDefault: false
            );

            if (!culture.IsNullOrEmpty())
            {
                return new ProviderCultureResult(culture, culture);
            }

            var result = await GetResultOrNull(httpContext, CookieProvider) ??
                         await GetResultOrNull(httpContext, HeaderProvider);

            if (result == null || !result.Cultures.Any())
            {
                return null;
            }

            //Try to set user's language setting from cookie if available.
            await settingManager.ChangeSettingForUserAsync(
                abpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                result.Cultures.First().Value
            );

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
