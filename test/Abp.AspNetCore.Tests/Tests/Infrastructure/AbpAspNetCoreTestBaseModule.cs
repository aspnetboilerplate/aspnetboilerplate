using System.Reflection;
using Abp.Modules;

namespace Abp.AspNetCore.Tests.Infrastructure
{
    [DependsOn(typeof(AbpAspNetCoreModule))]
    public class AbpAspNetCoreTestBaseModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.EventBus.UseDefaultEventBus = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}