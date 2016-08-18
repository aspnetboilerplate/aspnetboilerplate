using System;

using Abp.BackgroundJobs;
using Abp.Configuration.Startup;
using Abp.Dependency;

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

        /// <summary>
        ///     Configures to use Quartz for background job management.
        /// </summary>
        public static void UseQuartz(this IBackgroundJobConfiguration backgroundJobConfiguration, Action<IAbpQuartzConfiguration> configureAction = null)
        {
            backgroundJobConfiguration.AbpConfiguration.IocManager.RegisterIfNot<IQuartzScheduleJobManager, QuartzScheduleJobManager>();
            configureAction?.Invoke(backgroundJobConfiguration.AbpConfiguration.Modules.AbpQuartz());
        }
    }
}