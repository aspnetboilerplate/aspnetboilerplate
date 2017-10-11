using Abp.Dependency;
using Abp.Extensions;
using Quartz;
using Quartz.Spi;

namespace Abp.Quartz.Quartz
{
    public class AbpQuartzJobFactory : IJobFactory
    {
        private readonly IIocResolver _iocResolver;

        public AbpQuartzJobFactory(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _iocResolver.Resolve(bundle.JobDetail.JobType).As<IJob>();
        }

        public void ReturnJob(IJob job)
        {
            _iocResolver.Release(job);
        }
    }
}