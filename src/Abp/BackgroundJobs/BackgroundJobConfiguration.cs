using System;
using Abp.Configuration.Startup;

namespace Abp.BackgroundJobs
{
    internal class BackgroundJobConfiguration : IBackgroundJobConfiguration
    {
        public bool IsJobExecutionEnabled { get; set; }

        [Obsolete("Use UserTokenExpirationPeriod instead.")]
        public int? CleanUserTokenPeriod { get; set; }

        public TimeSpan? UserTokenExpirationPeriod { get; set; }

        public IAbpStartupConfiguration AbpConfiguration { get; private set; }

        public BackgroundJobConfiguration(IAbpStartupConfiguration abpConfiguration)
        {
            AbpConfiguration = abpConfiguration;

            IsJobExecutionEnabled = true;
        }
    }
}
