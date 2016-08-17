using System.Reflection;

using Abp.Modules;
using Abp.Quartz.Quartz;
using Abp.Quartz.Quartz.Configuration;

using AbpQuartzDemo.HelloJob;

namespace AbpQuartzDemo.ConsoleApp
{
    [DependsOn(
        typeof(AbpQuartzModule),
        typeof(AbpQuartzDemoHelloJobModule)
        )]
    public class AbpQuartzDemoConsoleAppModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.UseQuartz();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}