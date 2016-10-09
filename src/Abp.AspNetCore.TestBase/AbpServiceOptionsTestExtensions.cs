using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.TestBase.Runtime.Session;

namespace Abp.AspNetCore.TestBase
{
    public static class AbpServiceOptionsTestExtensions
    {
        public static void SetupTest(this AbpServiceOptions options)
        {
            options.IocManager = new IocManager();
            options.IocManager.RegisterIfNot<IAbpSession, TestAbpSession>();
        }
    }
}