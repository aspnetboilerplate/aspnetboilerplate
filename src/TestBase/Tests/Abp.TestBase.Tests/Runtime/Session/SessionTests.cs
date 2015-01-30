using Abp.Runtime.Session;
using Shouldly;
using Xunit;

namespace Abp.TestBase.Tests.Runtime.Session
{
    public class SessionTests : AbpIntegratedTestBase
    {
        [Fact]
        public void Should_Be_All_Null_On_Startup()
        {
            AbpSession.UserId.ShouldBe(null);
            AbpSession.TenantId.ShouldBe(null);            
        }

        [Fact]
        public void Can_Change_Session_Variables()
        {
            AbpSession.UserId = 1;
            AbpSession.TenantId = 42;

            var resolvedAbpSession = LocalIocManager.Resolve<IAbpSession>();
            resolvedAbpSession.UserId.ShouldBe(1);
            resolvedAbpSession.TenantId.ShouldBe(42);
        }
    }
}
