using Abp.BackgroundJobs;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;

namespace Abp.Hangfire
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpHangfireAspNetCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.ReplaceService<IBackgroundJobManager, HangfireBackgroundJobManager>(DependencyLifeStyle.Singleton);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpHangfireAspNetCoreModule).GetAssembly());
        }
    }
}
