using System.Reflection;
using Abp.Modules;

namespace Abp.TestBase
{
    [DependsOn(typeof(AbpKernelModule))]
    public class TestBaseModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}