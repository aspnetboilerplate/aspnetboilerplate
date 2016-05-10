using Abp.Configuration.Startup;

namespace Abp.BackgroundJobs
{
    internal class BackgroundJobConfiguration : IBackgroundJobConfiguration
    {
        public BackgroundJobConfiguration(IAbpStartupConfiguration abpConfiguration)
        {
            AbpConfiguration = abpConfiguration;

            IsJobExecutionEnabled = true;
        }

        public bool IsJobExecutionEnabled { get; set; }

        public IAbpStartupConfiguration AbpConfiguration { get; }
    }
}