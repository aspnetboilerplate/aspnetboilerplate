using Abp.Configuration.Startup;

namespace Abp.Zero.AspNetCore
{
    /// <summary>
    /// Extension methods for module zero configurations.
    /// </summary>
    public static class ModuleZeroAspNetCoreConfigurationExtensions
    {
        /// <summary>
        /// Configures ABP Zero AspNetCore module.
        /// </summary>
        /// <returns></returns>
        public static IAbpZeroAspNetCoreConfiguration ZeroAspNetCore(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration.Get<IAbpZeroAspNetCoreConfiguration>();
        }
    }
}