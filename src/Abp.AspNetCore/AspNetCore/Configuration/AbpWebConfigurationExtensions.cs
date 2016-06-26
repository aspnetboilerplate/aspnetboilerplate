using Abp.Configuration.Startup;

namespace Abp.AspNetCore.Configuration
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ABP Web module.
    /// </summary>
    public static class AbpWebConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP Web module.
        /// </summary>
        public static IAbpAspNetCoreConfiguration AbpAspNetCore(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.AspNetCore", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpAspNetCoreConfiguration>());
        }
    }
}