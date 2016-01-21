using Hangfire;

namespace Abp.Hangfire.Configuration
{
    public interface IAbpHangfireConfiguration
    {
        BackgroundJobServer Server { get; set; }
    }
}