using Quartz;
using Quartz.Impl;

namespace Abp.Quartz.Quartz.Configuration
{
    public class AbpQuartzConfiguration : IAbpQuartzConfiguration
    {
        public IScheduler Scheduler => StdSchedulerFactory.GetDefaultScheduler();
    }
}