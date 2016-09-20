using Abp.BackgroundJobs;
using Abp.Modules;
using Hangfire;

namespace Abp.Hangfire.Configuration
{
    /// <summary>
    /// Used to configure Hangfire.
    /// </summary>
    public interface IAbpHangfireConfiguration
    {
        /// <summary>
        /// Gets or sets the Hanfgire's <see cref="BackgroundJobServer"/> object.
        /// Important: This is null in <see cref="AbpModule.PreInitialize"/>. You can create and set it to customize it's creation.
        /// If you don't set it, it's automatically set in <see cref="AbpModule.PreInitialize"/> by Abp.HangFire module with it's default constructor
        /// if background job execution is enabled (see <see cref="IBackgroundJobConfiguration.IsJobExecutionEnabled"/>).
        /// So, if you create it yourself, it's your responsibility to check if background job execution is enabled (see <see cref="IBackgroundJobConfiguration.IsJobExecutionEnabled"/>).
        /// </summary>
        BackgroundJobServer Server { get; set; }

        /// <summary>
        /// A reference to Hangfire's global configuration.
        /// </summary>
        IGlobalConfiguration GlobalConfiguration { get; }
    }
}