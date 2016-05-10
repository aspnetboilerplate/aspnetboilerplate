using System;
using Abp.BackgroundJobs;
using Abp.Configuration.Startup;
using Abp.Dependency;

namespace Abp.Hangfire.Configuration
{
    public static class AbpHangfireConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure ABP Hangfire module.
        /// </summary>
        public static IAbpHangfireConfiguration AbpHangfire(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.Hangfire",
                () => configurations.AbpConfiguration.IocManager.Resolve<IAbpHangfireConfiguration>());
        }

        /// <summary>
        ///     Configures to use Hangfire for background job management.
        /// </summary>
        public static void UseHangfire(this IBackgroundJobConfiguration backgroundJobConfiguration,
            Action<IAbpHangfireConfiguration> configureAction)
        {
            backgroundJobConfiguration.AbpConfiguration.IocManager
                .RegisterIfNot<IBackgroundJobManager, HangfireBackgroundJobManager>();
            configureAction(backgroundJobConfiguration.AbpConfiguration.Modules.AbpHangfire());
        }
    }
}