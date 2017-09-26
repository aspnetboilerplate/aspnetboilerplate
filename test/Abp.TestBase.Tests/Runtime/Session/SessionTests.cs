using Abp.Configuration.Startup;
using Abp.Runtime.Session;
using Shouldly;
using Xunit;

namespace Abp.TestBase.Tests.Runtime.Session
{
    public class SessionTests : AbpIntegratedTestBase<AbpKernelModule>
    {
        [Fact]
        public void Should_Be_Default_On_Startup()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = false;

            AbpSession.UserId.ShouldBe(null);
            AbpSession.TenantId.ShouldBe(1);

            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            AbpSession.UserId.ShouldBe(null);
            AbpSession.TenantId.ShouldBe(null);
        }

        [Fact]
        public void Can_Change_Session_Variables()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            AbpSession.UserId = 1;
            AbpSession.TenantId = 42;

            var resolvedAbpSession = LocalIocManager.Resolve<IAbpSession>();

            resolvedAbpSession.UserId.ShouldBe(1);
            resolvedAbpSession.TenantId.ShouldBe(42);

            Resolve<IMultiTenancyConfig>().IsEnabled = false;

            AbpSession.UserId.ShouldBe(1);
            AbpSession.TenantId.ShouldBe(1);
        }
    }
}
