using Abp.Configuration.Startup;

namespace Abp.EntityFrameworkCore.Configuration
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ABP EntityFramework Core module.
    /// </summary>
    public static class AbpEfCoreConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP EntityFramework Core module.
        /// </summary>
        public static IAbpEfCoreConfiguration AbpEfCore(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpEfCoreConfiguration>();
        }
    }
}