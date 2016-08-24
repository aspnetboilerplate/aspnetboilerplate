using Quartz;

namespace Abp.Quartz.Quartz.Configuration
{
    public interface IAbpQuartzConfiguration
    {
        IScheduler Scheduler { get;}
    }
}