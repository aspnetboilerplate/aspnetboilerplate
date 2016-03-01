using Adorable.Configuration.Startup;

namespace Adorable.WebApi.OData.Configuration
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure Adorable.Web.Api.OData module.
    /// </summary>
    public static class AbpWebApiODataConfigurationExtensions
    {
        /// <summary>
        /// Used to configure Adorable.Web.Api.OData module.
        /// </summary>
        public static IAbpWebApiODataModuleConfiguration AbpWebApiOData(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Adorable.Web.Api.OData", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpWebApiODataModuleConfiguration>());
        }
    }
}