using System.Reflection;
using Abp.Dependency;
using Abp.Modules;
using Abp.Quartz.Configuration;
using Abp.Threading;
using Abp.Threading.BackgroundWorkers;
using Quartz;

namespace Abp.Quartz
{
    [DependsOn(typeof (AbpKernelModule))]
    public class AbpQuartzModule : AbpModule
    {
        private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();
        
        public override void PreInitialize()
        {
            IocManager.Register<IAbpQuartzConfiguration, AbpQuartzConfiguration>();

            OneTimeRunner.Run(() =>
            {
                Configuration.Modules.AbpQuartz().Scheduler.JobFactory = new AbpQuartzJobFactory(IocManager); 
            });
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
