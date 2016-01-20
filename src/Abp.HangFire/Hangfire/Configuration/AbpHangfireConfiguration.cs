using Hangfire;

namespace Abp.Hangfire.Configuration
{
    public class AbpHangfireConfiguration : IAbpHangfireConfiguration
    {
        public BackgroundJobServer Server { get; set; }
    }
}