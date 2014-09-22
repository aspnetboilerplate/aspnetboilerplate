using Abp.Startup.Configuration;

namespace Abp.Web.Startup
{
    /// <summary>
    /// 
    /// </summary>
    public static class ModuleConfigurationExtensions
    {
        public static AbpWebModuleConfiguration AbpWebModule(this AbpModuleConfigurations configurations)
        {
            return AbpWebModuleConfiguration.Instance;
        }
    }
}