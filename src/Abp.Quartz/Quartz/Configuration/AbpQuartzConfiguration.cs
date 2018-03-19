using Abp.Dependency;
using Quartz;
using Quartz.Impl;

namespace Abp.Quartz.Configuration
{
    public class AbpQuartzConfiguration : IAbpQuartzConfiguration
    {
        static object lockobj = new object();
        public IScheduler Scheduler
        {
            get
            {
                lock (lockobj)
                {
                    return StdSchedulerFactory.GetDefaultScheduler().Result;
                }
            }
        }

    }
}