using Abp.Modules;
using System.Reflection;

namespace Abp.TestBase
{
    [DependsOn(typeof(AbpKernelModule))]
    public class TestBaseModule : AbpModule
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