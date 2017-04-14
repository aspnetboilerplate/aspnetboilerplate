using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Abp.Hangfire
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpHangfireAspNetCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpHangfireAspNetCoreModule).GetAssembly());
        }
    }
}
