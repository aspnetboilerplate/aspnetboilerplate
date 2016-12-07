using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Abp.Extensions;

namespace Abp.AspNetCore.Localization
{
    public class SettingRequestCultureProvider : RequestCultureProvider
    {
        private readonly ISettingManager _settingManager;

        public SettingRequestCultureProvider(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var defaultLanguage = _settingManager.GetSettingValue(LocalizationSettingNames.DefaultLanguage);
            if (defaultLanguage.IsNullOrEmpty() || !GlobalizationHelper.IsValidCultureCode(defaultLanguage))
            {
                return Task.FromResult((ProviderCultureResult) null);
            }

            var providerResultCulture = new ProviderCultureResult(defaultLanguage, defaultLanguage);
            return Task.FromResult(providerResultCulture);
        }
    }
}
