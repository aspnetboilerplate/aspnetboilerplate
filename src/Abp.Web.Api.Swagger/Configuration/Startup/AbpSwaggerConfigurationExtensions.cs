using Abp.WebApi.Swagger.Configuration;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure Abp.Web.Api.Swagger module.
    /// </summary>
    public static class AbpSwaggerConfigurationExtensions
    {
        /// <summary>
        /// Used to configure Abp.Web.Api.Swagger module.
        /// </summary>
        public static IAbpSwaggerModuleConfiguration AbpSwagger(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.WebApi.Swagger", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpSwaggerModuleConfiguration>());
        }
    }
}
