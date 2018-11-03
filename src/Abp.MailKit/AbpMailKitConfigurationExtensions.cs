using Abp.Configuration.Startup;

namespace Abp.MailKit
{
    public static class AbpMailKitConfigurationExtensions
    {
        public static IAbpMailKitConfiguration AbpMailKit(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpMailKitConfiguration>();
        }
    }
}