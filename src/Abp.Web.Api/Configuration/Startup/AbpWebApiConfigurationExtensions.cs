using Abp.WebApi.Configuration;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ABP Web module.
    /// </summary>
    public static class AbpWebApiConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP Web module.
        /// </summary>
        public static IAbpWebApiModuleConfiguration AbpWebApi(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.Web.Api", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpWebApiModuleConfiguration>());
        }
    }
}