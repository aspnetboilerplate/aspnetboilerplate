using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Application.Navigation;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.Timing.Timezone;
using Abp.Web.Models.AbpUserConfiguration;
using Abp.Web.Security.AntiForgery;
using System.Linq;
using Abp.Dependency;
using Abp.Extensions;

namespace Abp.Web.Configuration
{
    public class AbpUserConfigurationBuilder : ITransientDependency
    {
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly ILanguageManager _languageManager;
        private readonly ILocalizationManager _localizationManager;
        private readonly IFeatureManager _featureManager;
        private readonly IFeatureChecker _featureChecker;
        private readonly IPermissionManager _permissionManager;
        private readonly IUserNavigationManager _userNavigationManager;
        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly ISettingManager _settingManager;
        private readonly IAbpAntiForgeryConfiguration _abpAntiForgeryConfiguration;
        private readonly IAbpSession _abpSession;
        private readonly IPermissionChecker _permissionChecker;

        public AbpUserConfigurationBuilder(
            IMultiTenancyConfig multiTenancyConfig,
            ILanguageManager languageManager,
            ILocalizationManager localizationManager,
            IFeatureManager featureManager,
            IFeatureChecker featureChecker,
            IPermissionManager permissionManager,
            IUserNavigationManager userNavigationManager,
            ISettingDefinitionManager settingDefinitionManager,
            ISettingManager settingManager,
            IAbpAntiForgeryConfiguration abpAntiForgeryConfiguration,
            IAbpSession abpSession,
            IPermissionChecker permissionChecker)
        {
            _multiTenancyConfig = multiTenancyConfig;
            _languageManager = languageManager;
            _localizationManager = localizationManager;
            _featureManager = featureManager;
            _featureChecker = featureChecker;
            _permissionManager = permissionManager;
            _userNavigationManager = userNavigationManager;
            _settingDefinitionManager = settingDefinitionManager;
            _settingManager = settingManager;
            _abpAntiForgeryConfiguration = abpAntiForgeryConfiguration;
            _abpSession = abpSession;
            _permissionChecker = permissionChecker;
        }

        public async Task<AbpUserConfigurationDto> GetAll()
        {
            return new AbpUserConfigurationDto
            {
                MultiTenancy = GetUserMultiTenancyConfig(),
                Session = GetUserSessionConfig(),
                Localization = GetUserLocalizationConfig(),
                Features = await GetUserFeaturesConfig(),
                Auth = await GetUserAuthConfig(),
                Nav = await GetUserNavConfig(),
                Setting = await GetUserSettingConfig(),
                Clock = GetUserClockConfig(),
                Timing = await GetUserTimingConfig(),
                Security = GetUserSecurityConfig()
            };
        }

        private AbpMultiTenancyConfigDto GetUserMultiTenancyConfig()
        {
            return new AbpMultiTenancyConfigDto
            {
                IsEnabled = _multiTenancyConfig.IsEnabled
            };
        }

        private AbpUserSessionConfigDto GetUserSessionConfig()
        {
            return new AbpUserSessionConfigDto
            {
                UserId = _abpSession.UserId,
                TenantId = _abpSession.TenantId,
                ImpersonatorUserId = _abpSession.ImpersonatorUserId,
                ImpersonatorTenantId = _abpSession.ImpersonatorTenantId,
                MultiTenancySide = _abpSession.MultiTenancySide
            };
        }

        private AbpUserLocalizationConfigDto GetUserLocalizationConfig()
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture;
            var languages = _languageManager.GetLanguages();

            var config = new AbpUserLocalizationConfigDto
            {
                CurrentCulture = new AbpUserCurrentCultureConfigDto
                {
                    Name = currentCulture.Name,
                    DisplayName = currentCulture.DisplayName
                },
                Languages = languages.ToList()
            };

            if (languages.Count > 0)
            {
                config.CurrentLanguage = _languageManager.CurrentLanguage;
            }

            var sources = _localizationManager.GetAllSources().OrderBy(s => s.Name).ToArray();
            config.Sources = sources.Select(s => new AbpLocalizationSourceDto
            {
                Name = s.Name,
                Type = s.GetType().Name
            }).ToList();

            config.Values = new Dictionary<string, Dictionary<string, string>>();
            foreach (var source in sources)
            {
                var stringValues = source.GetAllStrings(currentCulture).OrderBy(s => s.Name).ToList();
                var stringDictionary = stringValues
                    .ToDictionary(_ => _.Name, _ => _.Value);
                config.Values.Add(source.Name, stringDictionary);
            }

            return config;
        }

        private async Task<AbpUserFeatureConfigDto> GetUserFeaturesConfig()
        {
            var config = new AbpUserFeatureConfigDto()
            {
                AllFeatures = new Dictionary<string, AbpStringValueDto>()
            };

            var allFeatures = _featureManager.GetAll().ToList();

            if (_abpSession.TenantId.HasValue)
            {
                var currentTenantId = _abpSession.GetTenantId();
                foreach (var feature in allFeatures)
                {
                    var value = await _featureChecker.GetValueAsync(currentTenantId, feature.Name);
                    config.AllFeatures.Add(feature.Name, new AbpStringValueDto
                    {
                        Value = value
                    });
                }
            }
            else
            {
                foreach (var feature in allFeatures)
                {
                    config.AllFeatures.Add(feature.Name, new AbpStringValueDto
                    {
                        Value = feature.DefaultValue
                    });
                }
            }

            return config;
        }

        private async Task<AbpUserAuthConfigDto> GetUserAuthConfig()
        {
            var config = new AbpUserAuthConfigDto();

            var allPermissionNames = _permissionManager.GetAllPermissions(false).Select(p => p.Name).ToList();
            var grantedPermissionNames = new List<string>();

            if (_abpSession.UserId.HasValue)
            {
                foreach (var permissionName in allPermissionNames)
                {
                    if (await _permissionChecker.IsGrantedAsync(permissionName))
                    {
                        grantedPermissionNames.Add(permissionName);
                    }
                }
            }

            config.AllPermissions = allPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");
            config.GrantedPermissions = grantedPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");

            return config;
        }

        private async Task<AbpUserNavConfigDto> GetUserNavConfig()
        {
            var userMenus = await _userNavigationManager.GetMenusAsync(_abpSession.ToUserIdentifier());
            return new AbpUserNavConfigDto
            {
                Menus = userMenus.ToDictionary(userMenu => userMenu.Name, userMenu => userMenu)
            };
        }

        private async Task<AbpUserSettingConfigDto> GetUserSettingConfig()
        {
            var config = new AbpUserSettingConfigDto
            {
                Values = new Dictionary<string, string>()
            };

            var settingDefinitions = _settingDefinitionManager
                .GetAllSettingDefinitions()
                .Where(sd => sd.IsVisibleToClients);

            foreach (var settingDefinition in settingDefinitions)
            {
                var settingValue = await _settingManager.GetSettingValueAsync(settingDefinition.Name);
                config.Values.Add(settingDefinition.Name, settingValue);
            }

            return config;
        }

        private AbpUserClockConfigDto GetUserClockConfig()
        {
            return new AbpUserClockConfigDto
            {
                Provider = Clock.Provider.GetType().Name.ToCamelCase()
            };
        }

        private async Task<AbpUserTimingConfigDto> GetUserTimingConfig()
        {
            var timezoneId = await _settingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);

            return new AbpUserTimingConfigDto
            {
                TimeZoneInfo = new AbpUserTimeZoneConfigDto
                {
                    Windows = new AbpUserWindowsTimeZoneConfigDto
                    {
                        TimeZoneId = timezoneId,
                        BaseUtcOffsetInMilliseconds = timezone.BaseUtcOffset.TotalMilliseconds,
                        CurrentUtcOffsetInMilliseconds = timezone.GetUtcOffset(Clock.Now).TotalMilliseconds,
                        IsDaylightSavingTimeNow = timezone.IsDaylightSavingTime(Clock.Now)
                    },
                    Iana = new AbpUserIanaTimeZoneConfigDto
                    {
                        TimeZoneId = TimezoneHelper.WindowsToIana(timezoneId)
                    }
                }
            };
        }

        private AbpUserSecurityConfigDto GetUserSecurityConfig()
        {
            return new AbpUserSecurityConfigDto()
            {
                AntiForgery = new AbpUserAntiForgeryConfigDto
                {
                    TokenCookieName = _abpAntiForgeryConfiguration.TokenCookieName,
                    TokenHeaderName = _abpAntiForgeryConfiguration.TokenHeaderName
                }
            };
        }
    }
}