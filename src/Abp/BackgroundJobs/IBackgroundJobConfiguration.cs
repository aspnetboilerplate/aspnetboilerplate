using Abp.Configuration.Startup;

namespace Abp.BackgroundJobs
{
    /// <summary>
    /// Used to configure background job system.
    /// </summary>
    public interface IBackgroundJobConfiguration
    {
        /// <summary>
        /// Used to enable/disable background job execution.
        /// </summary>
        bool IsJobExecutionEnabled { get; set; }

        /// <summary>
        /// Period in milliseconds.
        /// </summary>
        int? CleanUserTokenPeriod { get; set; }

        /// <summary>
        /// Gets the ABP configuration object.
        /// </summary>
        IAbpStartupConfiguration AbpConfiguration { get; }
    }
}