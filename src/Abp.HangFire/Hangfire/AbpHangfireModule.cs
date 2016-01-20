using System.Reflection;
using Abp.Hangfire.Configuration;
using Abp.Modules;
using Abp.Threading.BackgroundWorkers;
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
            //.UseSqlServerStorage("Default", options); // Here you can put any Connection String
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<IBackgroundWorkerManager>().Add(
                IocManager.Resolve<HangfireBackgroundJobManager>()
                );
        }
    }
}
