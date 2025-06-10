using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;

namespace Abp.OpenIddict.EntityFrameworkCore;

[DependsOn(typeof(AbpZeroCoreOpenIddictModule), typeof(AbpZeroCoreEntityFrameworkCoreModule))]
public class AbpZeroCoreOpenIddictEntityFrameworkCoreModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(
            typeof(AbpZeroCoreOpenIddictEntityFrameworkCoreModule).GetAssembly()
        );
    }
}