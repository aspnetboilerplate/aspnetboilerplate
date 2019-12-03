using System.Collections.Generic;

namespace Abp.Web.Models.AbpUserConfiguration
{
    public class AbpUserConfigurationDto
    {
        public AbpMultiTenancyConfigDto MultiTenancy { get; set; }

        public AbpUserSessionConfigDto Session { get; set; }

        public AbpUserLocalizationConfigDto Localization { get; set; }

        public AbpUserFeatureConfigDto Features { get; set; }

        public AbpUserAuthConfigDto Auth { get; set; }

        public AbpUserNavConfigDto Nav { get; set; }

        public AbpUserSettingConfigDto Setting { get; set; }

        public AbpUserClockConfigDto Clock { get; set; }

        public AbpUserTimingConfigDto Timing { get; set; }

        public AbpUserSecurityConfigDto Security { get; set; }

        public Dictionary<string, object> Custom { get; set; }
    }
}