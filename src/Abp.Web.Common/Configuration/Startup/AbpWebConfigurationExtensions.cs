using Abp.Web.Configuration;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ABP Web module.
    /// </summary>
    public static class AbpWebConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP Web module.
        /// </summary>
        public static IAbpWebModuleConfiguration AbpWeb(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpWebModuleConfiguration>();
        }
    }
}