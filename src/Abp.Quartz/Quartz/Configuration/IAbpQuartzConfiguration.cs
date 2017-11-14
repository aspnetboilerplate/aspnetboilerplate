using Quartz;

namespace Abp.Quartz.Configuration
{
    public interface IAbpQuartzConfiguration
    {
        IScheduler Scheduler { get;}
    }
}