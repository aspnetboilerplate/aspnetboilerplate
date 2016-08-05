using System.Reflection;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.AspNetCore.Configuration;

namespace Abp.AspNetCore.App
{
    [DependsOn(typeof(AbpAspNetCoreTestBaseModule))]
    public class AppModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration
                .Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    Assembly.GetExecutingAssembly()
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
