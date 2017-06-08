using Abp.Configuration.Startup;

namespace Abp.Web.Mvc.Configuration
{
    /// <summary>
    /// 定义扩展方法，以便<see cref =“IModuleConfigurations”/>允许配置Abp.Web.Api模块。
    /// </summary>
    public static class AbpMvcConfigurationExtensions
    {
        /// <summary>
        /// 用于配置Abp.Web.Api模块。
        /// </summary>
        public static IAbpMvcConfiguration AbpMvc(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpMvcConfiguration>();
        }
    }
}