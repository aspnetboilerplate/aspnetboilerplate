using System.Reflection;
using Abp.Modules;
using Abp.TestBase;

namespace Abp.Web.Common.Tests
{
    [DependsOn(typeof(AbpWebCommonModule), typeof(AbpTestBaseModule))]
    public class AbpWebCommonTestModule : AbpModule
    {
        public override void Initialize()
        {
            base.Initialize();

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
