using System;
using Abp.BackgroundJobs;
using Abp.Configuration.Startup;
using Abp.Dependency;

namespace Abp.Hangfire.Configuration
{
    public static class AbpHangfireConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP Hangfire module.
        /// </summary>
        public static IAbpHangfireConfiguration AbpHangfire(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpHangfireConfiguration>();
        }

        /// <summary>
        /// Configures to use Hangfire for background job management.
        /// </summary>
        public static void UseHangfire(this IBackgroundJobConfiguration backgroundJobConfiguration, Action<IAbpHangfireConfiguration> configureAction)
        {
            backgroundJobConfiguration.AbpConfiguration.ReplaceService<IBackgroundJobManager, HangfireBackgroundJobManager>();
            configureAction(backgroundJobConfiguration.AbpConfiguration.Modules.AbpHangfire());
        }
    }
}