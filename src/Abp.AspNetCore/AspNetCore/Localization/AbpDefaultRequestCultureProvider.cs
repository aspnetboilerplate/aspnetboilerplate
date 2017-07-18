using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.AspNetCore.Localization
{
    public class AbpDefaultRequestCultureProvider : RequestCultureProvider
    {
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var settingManager = httpContext.RequestServices.GetRequiredService<ISettingManager>();

            var culture = await settingManager.GetSettingValueAsync(LocalizationSettingNames.DefaultLanguage);

            if (culture.IsNullOrEmpty())
            {
                return null;
            }

            return new ProviderCultureResult(culture, culture);
        }
    }
}
