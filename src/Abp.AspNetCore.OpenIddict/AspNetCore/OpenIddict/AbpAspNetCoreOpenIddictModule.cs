using System.Reflection;
using Abp.Modules;
using Abp.OpenIddict;

namespace Abp.AspNetCore.OpenIddict;

[DependsOn(typeof(AbpAspNetCoreModule), typeof(AbpZeroCoreOpenIddictModule))]
public class AbpAspNetCoreOpenIddictModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}