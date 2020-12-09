using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Abp.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.MultiTenancy
{
    public class TenantCache_Tests : SampleAppTestBase
    {
        private readonly ITenantCache _tenantCache;
        private readonly IRepository<Tenant> _tenantRepository;

        public TenantCache_Tests()
        {
            _tenantCache = Resolve<ITenantCache>();
            _tenantRepository = Resolve<IRepository<Tenant>>();
        }

        [Fact]
        public void Should_Get_Tenant_By_Id()
        {
            //Act
            var tenant = _tenantCache.Get(1);

            //Assert
            tenant.TenancyName.ShouldBe(Tenant.DefaultTenantName);
        }

        [Fact]
        public void Should_Get_Tenant_By_TenancyName()
        {
            //Act
            var tenant = _tenantCache.GetOrNull(Tenant.DefaultTenantName);

            //Assert
            tenant.Id.ShouldBe(1);
        }

        [Fact]
        public void Should_Get_Null_Tenant_By_TenancyName_If_Not_Found()
        {
            //Act
            var tenant = _tenantCache.GetOrNull("unknown-tenancy-name");

            //Assert
            tenant.ShouldBeNull();
        }

        [Fact]
        public void Should_Refresh_Cache_If_Tenancy_Name_Changes()
        {
            //--- Get a known tenant from cache

            //Act
            var tenant = _tenantCache.GetOrNull(Tenant.DefaultTenantName);

            //Assert
            tenant.Id.ShouldBe(1);
            tenant.IsActive.ShouldBeTrue();

            //--- Change tenant name

            _tenantRepository.Update(tenant.Id, t =>
            {
                t.TenancyName = "Default-Changed";
                t.IsActive = false;
            });

            //--- Get again from cache

            //Can not get with old name
            tenant = _tenantCache.GetOrNull(Tenant.DefaultTenantName);
            tenant.ShouldBeNull();

            //Can get with new name
            tenant = _tenantCache.GetOrNull("Default-Changed");
            tenant.ShouldNotBeNull();
            tenant.Id.ShouldBe(1);
            tenant.IsActive.ShouldBeFalse();
        }

        [Fact]
        public async Task Should_Get_Tenant_By_Id_Async()
        {
            //Act
            var tenant = await _tenantCache.GetAsync(1);

            //Assert
            tenant.TenancyName.ShouldBe(Tenant.DefaultTenantName);
        }

        [Fact]
        public async Task  Should_Get_Tenant_By_TenancyName_Async()
        {
            //Act
            var tenant = await _tenantCache.GetOrNullAsync(Tenant.DefaultTenantName);

            //Assert
            tenant.Id.ShouldBe(1);
        }

        [Fact]
        public async Task Should_Get_Null_Tenant_By_TenancyName_If_Not_Found_Async()
        {
            //Act
            var tenant = await _tenantCache.GetOrNullAsync("unknown-tenancy-name");

            //Assert
            tenant.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Refresh_Cache_If_Tenancy_Name_Changes_Async()
        {
            //--- Get a known tenant from cache

            //Act
            var tenant = await _tenantCache.GetOrNullAsync(Tenant.DefaultTenantName);

            //Assert
            tenant.Id.ShouldBe(1);
            tenant.IsActive.ShouldBeTrue();

            //--- Change tenant name

            _tenantRepository.Update(tenant.Id, t =>
            {
                t.TenancyName = "Default-Changed";
                t.IsActive = false;
            });

            //--- Get again from cache

            //Can not get with old name
            tenant = await _tenantCache.GetOrNullAsync(Tenant.DefaultTenantName);
            tenant.ShouldBeNull();

            //Can get with new name
            tenant = await _tenantCache.GetOrNullAsync("Default-Changed");
            tenant.ShouldNotBeNull();
            tenant.Id.ShouldBe(1);
            tenant.IsActive.ShouldBeFalse();
        }
    }
}
