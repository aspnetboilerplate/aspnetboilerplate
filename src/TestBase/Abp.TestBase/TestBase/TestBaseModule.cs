using System.Reflection;
using Adorable.Modules;

namespace Adorable.TestBase
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