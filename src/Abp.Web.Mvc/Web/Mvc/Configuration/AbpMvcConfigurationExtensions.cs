using Abp.Configuration.Startup;

namespace Abp.Web.Mvc.Configuration
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure Abp.Web.Api module.
    /// </summary>
    public static class AbpMvcConfigurationExtensions
    {
        /// <summary>
        /// Used to configure Abp.Web.Api module.
        /// </summary>
        public static IAbpMvcConfiguration AbpMvc(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.Web.Mvc", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpMvcConfiguration>());
        }
    }
}