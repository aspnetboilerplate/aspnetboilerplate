using System.Reflection;
using Abp.Modules;

namespace Abp.TestBase.SampleApplication.Tests
{
    [DependsOn(typeof(SampleApplicationModule), typeof(AbpTestBaseModule))]
    public class SampleApplicationTestModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
