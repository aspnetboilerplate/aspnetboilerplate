using Abp.Collections;
using Abp.Modules;
using Abp.TestBase;

namespace Abp.EntityFramework.GraphDIff.Tests
{
    public class AbpEntityFrameworkGraphDiffTestBase : AbpIntegratedTestBase
    {
        protected override void AddModules(ITypeList<AbpModule> modules)
        {
            base.AddModules(modules);
            modules.Add<AbpEntityFrameworkGraphDiffTestModule>();
        }
    }
}
