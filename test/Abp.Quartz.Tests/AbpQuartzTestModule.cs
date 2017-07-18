using System.Reflection;

using Abp.Modules;
using Abp.Quartz.Quartz;

namespace Abp.Quartz.Tests
{
    [DependsOn(typeof(AbpQuartzModule))]
    public class AbpQuartzTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.IsJobExecutionEnabled = true;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
