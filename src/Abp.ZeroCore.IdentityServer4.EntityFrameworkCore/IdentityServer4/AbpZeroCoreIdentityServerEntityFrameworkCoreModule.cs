using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;

namespace Abp.IdentityServer4
{
    [DependsOn(typeof(AbpZeroCoreIdentityServerModule), typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class AbpZeroCoreIdentityServerEntityFrameworkCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpZeroCoreIdentityServerEntityFrameworkCoreModule).GetAssembly());
        }
    }
}
