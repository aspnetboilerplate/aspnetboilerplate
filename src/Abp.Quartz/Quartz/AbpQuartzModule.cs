using System.Reflection;
using Abp.Dependency;
using Abp.Modules;
using Abp.Quartz.Quartz.Configuration;
using Abp.Threading.BackgroundWorkers;
using Quartz;

namespace Abp.Quartz.Quartz
{
    [DependsOn(typeof (AbpKernelModule))]
    public class AbpQuartzModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpQuartzConfiguration, AbpQuartzConfiguration>();

            Configuration.Modules.AbpQuartz().Scheduler.JobFactory = new AbpQuartzJobFactory(IocManager);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.RegisterIfNot<IJobListener, AbpQuartzJobListener>();

            Configuration.Modules.AbpQuartz().Scheduler.ListenerManager.AddJobListener(IocManager.Resolve<IJobListener>());

            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                IocManager.Resolve<IBackgroundWorkerManager>().Add(IocManager.Resolve<IQuartzScheduleJobManager>());
            }
        }
    }
}
