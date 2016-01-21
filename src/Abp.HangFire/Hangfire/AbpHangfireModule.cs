using System.Reflection;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Hangfire.Configuration;
using Abp.Modules;
using Hangfire;

namespace Abp.Hangfire
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpHangfireModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpHangfireConfiguration, AbpHangfireConfiguration>();

            GlobalConfiguration.Configuration
                .UseWindsorJobActivator(IocManager);
        }

        public override void Initialize()
        {
            IocManager.RegisterIfNot<IBackgroundJobManager, HangfireBackgroundJobManager>(); //TODO: should not be needed.
            
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
