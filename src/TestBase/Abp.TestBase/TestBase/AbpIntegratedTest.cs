using System.Reflection;
using Abp.Dependency;
using Abp.TestBase.Runtime.Session;

namespace Abp.TestBase
{
    public abstract class AbpIntegratedTest
    {
        public IIocManager LocalIocManager { get; private set; }

        public TestAbpSession AbpSession { get; set; }

        protected AbpIntegratedTest()
        {
            LocalIocManager = new IocManager();

            LocalIocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());

            LocalIocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            AbpSession = LocalIocManager.Resolve<TestAbpSession>();
        }
    }
}
