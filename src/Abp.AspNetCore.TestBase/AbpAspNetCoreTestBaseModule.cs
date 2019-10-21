using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;

namespace Abp.AspNetCore.TestBase
{
    [DependsOn(typeof(AbpTestBaseModule), typeof(AbpAspNetCoreModule))]
    public class AbpAspNetCoreTestBaseModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpAspNetCoreTestBaseModule).GetAssembly());
        }
    }
}