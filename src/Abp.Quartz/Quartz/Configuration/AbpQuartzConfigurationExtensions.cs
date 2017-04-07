using Abp.Configuration.Startup;

namespace Abp.Quartz.Quartz.Configuration
{
    public static class AbpQuartzConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure ABP Quartz module.
        /// </summary>
        public static IAbpQuartzConfiguration AbpQuartz(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpQuartzConfiguration>();
        }
    }
}
