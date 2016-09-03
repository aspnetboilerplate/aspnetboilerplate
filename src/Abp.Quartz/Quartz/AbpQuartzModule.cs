using System.Reflection;

using Abp.Dependency;
using Abp.Modules;
using Abp.Quartz.Quartz.Configuration;
using Abp.Threading.BackgroundWorkers;

using Quartz;

namespace Abp.Quartz.Quartz
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpQuartzModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpQuartzConfiguration, AbpQuartzConfiguration>();
            IocManager.RegisterIfNot<IJobListener, AbpQuartzJobListener>();

            Configuration.Modules
                         .AbpQuartz()
                         .Scheduler
                         .JobFactory = new AbpQuartzWindsorFactory(IocManager);

            Configuration.Modules
                         .AbpQuartz()
                         .Scheduler
                         .ListenerManager.AddJobListener(IocManager.Resolve<IJobListener>());
        }
                
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                var workerManager = IocManager.Resolve<IBackgroundWorkerManager>();
                workerManager.Add(IocManager.Resolve<IQuartzScheduleJobManager>());
            }
        }
    }
}