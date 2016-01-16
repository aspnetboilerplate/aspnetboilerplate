using Abp.WebApi.Swagger.Configuration;

namespace Abp.Configuration.Startup
{
    public static class AbpSwaggerConfigurationExtensions
    {
        public static IAbpSwaggerModuleConfiguration AbpWebApi(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.WebApi.Swagger", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpSwaggerModuleConfiguration>());
        }
    }
}
