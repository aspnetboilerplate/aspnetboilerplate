using Abp.BackgroundJobs;
using Abp.Configuration.Startup;

namespace Abp.Hangfire.Configuration
{
    public static class AbpHangfireConfigurationExtensions
    {
        /// <summary>
        /// Configures to use Hangfire for background job management.
        /// </summary>
        public static void UseHangfire(this IBackgroundJobConfiguration backgroundJobConfiguration)
        {
            backgroundJobConfiguration.AbpConfiguration.ReplaceService<IBackgroundJobManager, HangfireBackgroundJobManager>();
        }
    }
}