using Abp.Configuration.Startup;

namespace Abp.AspNetCore.OData.Configuration
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure Abp.AspNetCore.OData module.
    /// </summary>
    public static class AbpAspNetCoreODataConfigurationExtensions
    {
        /// <summary>
        /// Used to configure Abp.AspNetCore.OData module.
        /// </summary>
        public static IAbpAspNetCoreODataModuleConfiguration AbpAspNetCoreOData(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpAspNetCoreODataModuleConfiguration>();
        }
    }
}
