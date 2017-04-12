using Abp.Configuration.Startup;

namespace Abp.MailKit
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ABP ASP.NET Core module.
    /// </summary>
    public static class AbpMailKitConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP ASP.NET Core module.
        /// </summary>
        public static IAbpMailKitConfiguration AbpMailKit(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpMailKitConfiguration>();
        }
    }
}