using Abp.Configuration.Startup;

namespace Abp.BackgroundJobs
{
    internal class BackgroundJobConfiguration : IBackgroundJobConfiguration
    {
        public bool IsJobExecutionEnabled { get; set; }

        public int? CleanUserTokenPeriod { get; set; }

        public IAbpStartupConfiguration AbpConfiguration { get; private set; }

        public BackgroundJobConfiguration(IAbpStartupConfiguration abpConfiguration)
        {
            AbpConfiguration = abpConfiguration;

            IsJobExecutionEnabled = true;
        }
    }
}