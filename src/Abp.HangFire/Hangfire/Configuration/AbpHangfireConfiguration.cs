using Hangfire;
using HangfireGlobalConfiguration = Hangfire.GlobalConfiguration;

namespace Abp.Hangfire.Configuration
{
    public class AbpHangfireConfiguration : IAbpHangfireConfiguration
    {
        public BackgroundJobServer Server { get; set; }

        public IGlobalConfiguration GlobalConfiguration => HangfireGlobalConfiguration.Configuration;
    }
}