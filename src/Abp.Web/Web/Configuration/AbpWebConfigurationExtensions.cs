using Abp.Startup.Configuration;

namespace Abp.Web.Configuration
{
    /// <summary>
    /// Defines extension methods to <see cref="IAbpModuleConfigurations"/> to allow to configure ABP Web module.
    /// </summary>
    public static class AbpWebConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP Web module.
        /// </summary>
        public static IAbpWebModuleConfiguration AbpWeb(this IAbpModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.Web", () => new AbpWebModuleConfiguration());
        }
    }
}