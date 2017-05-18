using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Abp.Localization;

namespace Abp.AspNetCore.Localization
{
    public class UserRequestCultureProvider : RequestCultureProvider
    {
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

            if (culture == null)
            {
                return null;
            }

            return new ProviderCultureResult(culture, culture);
        }
    }
}
