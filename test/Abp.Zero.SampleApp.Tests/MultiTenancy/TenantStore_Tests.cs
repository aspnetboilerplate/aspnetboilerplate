using Abp.MultiTenancy;
using Abp.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.MultiTenancy
{
    public class TenantStore_Tests : SampleAppTestBase
    {
        private readonly ITenantStore _tenantStore;

        public TenantStore_Tests()
        {
            _tenantStore = Resolve<ITenantStore>();
        }


        [Fact]
        public void Should_Get_Tenant_By_Id()
        {
            //Act
            var tenant = _tenantStore.Find(1);

            //Assert
            Assert.NotNull(tenant);
            tenant.TenancyName.ShouldBe(Tenant.DefaultTenantName);
        }
        
        [Fact]
        public void Should_Get_Tenant_By_Name()
        {
            //Act
            var tenant = _tenantStore.Find(Tenant.DefaultTenantName);

            //Assert
            Assert.NotNull(tenant);
            tenant.Id.ShouldBe(1);
        }

        [Fact]
        public void Should_Not_Get_Unknown_Tenant()
        {
            Assert.Null(_tenantStore.Find("unknown-tenancy-name"));
        }
    }
}