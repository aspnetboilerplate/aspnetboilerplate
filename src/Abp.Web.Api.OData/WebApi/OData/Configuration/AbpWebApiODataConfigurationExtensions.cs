using Abp.Configuration.Startup;

namespace Abp.WebApi.OData.Configuration
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure Abp.Web.Api.OData module.
    /// </summary>
    public static class AbpWebApiODataConfigurationExtensions
    {
        /// <summary>
        /// Used to configure Abp.Web.Api.OData module.
        /// </summary>
        public static IAbpWebApiODataModuleConfiguration AbpWebApiOData(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpWebApiODataModuleConfiguration>();
        }
    }
}