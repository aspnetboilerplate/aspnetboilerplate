using Abp.Collections;
using Abp.Modules;

namespace Abp.AspNetCore.TestBase
{
    public class TestOptions
    {
        public ITypeList<AbpModule> Modules { get; private set; }

        public TestOptions()
        {
            Modules = new TypeList<AbpModule>();
        }
    }
}