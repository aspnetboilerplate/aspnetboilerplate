using Abp.Dependency;
using Abp.Extensions;

using Quartz;
using Quartz.Spi;

namespace Abp.Quartz.Quartz
{
    public class AbpQuartzWindsorFactory : IJobFactory
    {
        private readonly IIocResolver iocResolver;

        public AbpQuartzWindsorFactory(IIocResolver iocResolver)
        {
            this.iocResolver = iocResolver;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return iocResolver.Resolve(bundle.JobDetail.JobType).As<IJob>();
        }

        public void ReturnJob(IJob job)
        {
            iocResolver.Release(job);
        }
    }
}