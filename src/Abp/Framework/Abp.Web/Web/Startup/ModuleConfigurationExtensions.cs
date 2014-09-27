using Abp.Startup.Configuration;

namespace Abp.Web.Startup
{
    /// <summary>
    /// 
    /// </summary>
    public static class ModuleConfigurationExtensions
    {
        public static IAbpWebModuleConfiguration AbpWeb(this IAbpModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.Web", () => new AbpWebModuleConfiguration());
        }
    }
}