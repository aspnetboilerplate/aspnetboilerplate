using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;

namespace Abp.IdentityServer4vNext
{
    [DependsOn(typeof(AbpZeroCoreIdentityServervNextModule), typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class AbpZeroCoreIdentityServervNextEntityFrameworkCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpZeroCoreIdentityServervNextEntityFrameworkCoreModule).GetAssembly());
        }
    }
}
