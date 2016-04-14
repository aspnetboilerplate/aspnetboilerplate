using Abp.Configuration.Startup;
using Abp.Runtime.Session;
using Shouldly;
using System;
using Xunit;

namespace Abp.TestBase.Tests.Runtime.Session
{
    public class SessionTests : AbpIntegratedTestBase
    {
        [Fact]
        public void Should_Be_Default_On_Startup()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = false;

            AbpSession.UserId.ShouldBe(null);
            AbpSession.TenantId.ShouldBe(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"));

            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            AbpSession.UserId.ShouldBe(null);
            AbpSession.TenantId.ShouldBe(null);
        }

        [Fact]
        public void Can_Change_Session_Variables()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            AbpSession.UserId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001");
            AbpSession.TenantId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000042");

            var resolvedAbpSession = LocalIocManager.Resolve<IAbpSession>();

            resolvedAbpSession.UserId.ShouldBe(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"));
            resolvedAbpSession.TenantId.ShouldBe(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000042"));

            Resolve<IMultiTenancyConfig>().IsEnabled = false;

            AbpSession.UserId.ShouldBe(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"));
            AbpSession.TenantId.ShouldBe(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"));
        }
    }
}