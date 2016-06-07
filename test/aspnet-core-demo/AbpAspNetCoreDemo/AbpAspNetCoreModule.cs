using System.Reflection;
using Abp.Modules;
using AbpAspNetCoreDemo.Core;

namespace AbpAspNetCoreDemo
{
    [DependsOn(typeof(AbpAspNetCoreDemoCoreModule))]
    public class AbpAspNetCoreDemoModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}