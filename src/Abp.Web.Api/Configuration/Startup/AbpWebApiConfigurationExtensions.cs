using Adorable.WebApi.Configuration;

namespace Adorable.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure Adorable.Web.Api module.
    /// </summary>
    public static class AbpWebApiConfigurationExtensions
    {
        /// <summary>
        /// Used to configure Adorable.Web.Api module.
        /// </summary>
        public static IAbpWebApiModuleConfiguration AbpWebApi(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Adorable.Web.Api", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpWebApiModuleConfiguration>());
        }
    }
}