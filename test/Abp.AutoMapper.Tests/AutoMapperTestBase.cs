using Abp.Collections;
using Abp.Modules;
using Abp.TestBase;

namespace Abp.AutoMapper.Tests
{
    public class AutoMapperTestBase : AbpIntegratedTestBase
    {
        protected override void AddModules(ITypeList<AbpModule> modules)
        {
            base.AddModules(modules);
            modules.Add<AutoMapperTestModule>();
        }
    }
}