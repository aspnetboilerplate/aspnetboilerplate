using Abp.Collections;

namespace Abp.Configuration.Startup
{
    internal class SettingsConfiguration : ISettingsConfiguration
    {
        public SettingsConfiguration()
        {
            Providers = new TypeList<SettingProvider>();
        }

        public ITypeList<SettingProvider> Providers { get; }
    }
}