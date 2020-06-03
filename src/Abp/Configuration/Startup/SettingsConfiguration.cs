using Abp.Collections;

namespace Abp.Configuration.Startup
{
    internal class SettingsConfiguration : ISettingsConfiguration
    {
        public SettingEncryptionConfiguration SettingEncryptionConfiguration { get; private set; }
        
        public ITypeList<SettingProvider> Providers { get; private set; }

        public SettingsConfiguration()
        {
            Providers = new TypeList<SettingProvider>();
            SettingEncryptionConfiguration = new SettingEncryptionConfiguration();
        }
    }
}