using System.Reflection;
using Abp.AspNetCore.TestBase;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mocks;
using Abp.Auditing;

namespace Abp.AspNetCore.App
{
    [DependsOn(typeof(AbpAspNetCoreTestBaseModule))]
    public class AppModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            Configuration.ReplaceService<IAuditingStore, MockAuditingStore>();

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
