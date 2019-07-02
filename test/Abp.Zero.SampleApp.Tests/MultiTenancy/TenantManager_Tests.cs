using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Domain.Uow;
using Abp.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.MultiTenancy
{
    public class TenantManager_Tests : SampleAppTestBase
    {
        private readonly TenantManager _tenantManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IFeatureChecker _featureChecker;

        public TenantManager_Tests()
        {
            _tenantManager = Resolve<TenantManager>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            _featureChecker = Resolve<IFeatureChecker>();
        }

        [Fact]
        public async Task Should_Not_Create_Duplicate_Tenant()
        {
            await _tenantManager.CreateAsync(new Tenant("Tenant-X", "Tenant X"));
            
            //Trying to re-create with same tenancy name

            await Assert.ThrowsAnyAsync<AbpException>(async () =>
            {
                await _tenantManager.CreateAsync(new Tenant("Tenant-X", "Tenant X"));
            });
        }


        [Fact]
        public async Task SetFeatureValue_Test()
        {
            var tenant = new Tenant("TestTenant", "TestTenant");
            await _tenantManager.CreateAsync(tenant);

            using (var uow = _unitOfWorkManager.Begin())
            {
                await _tenantManager.SetFeatureValueAsync(tenant.Id, "MyBoolFeature", "true");
                await _unitOfWorkManager.Current.SaveChangesAsync();

                (await _featureChecker.IsEnabledAsync(tenant.Id, "MyBoolFeature")).ShouldBeTrue();

                await uow.CompleteAsync();
            }
        }

    }
}
