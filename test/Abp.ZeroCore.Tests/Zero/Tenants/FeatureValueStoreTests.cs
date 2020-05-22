using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Abp.ZeroCore.SampleApp.Application;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.Tenants
{
    public class FeatureValueStoreTests : AbpZeroTestBase
    {
        private readonly ICacheManager _cacheManager;
        private readonly FeatureValueStore _featureValueStore;
        private readonly IRepository<TenantFeatureSetting, long> _tenantFeatureRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public FeatureValueStoreTests()
        {
            _cacheManager = Resolve<ICacheManager>();
            _featureValueStore = Resolve<FeatureValueStore>();
            _tenantFeatureRepository = Resolve<IRepository<TenantFeatureSetting, long>>();
            _tenantRepository = Resolve<IRepository<Tenant>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public void GetTenantFeatureCacheItem_ShouldEnableFilterMayHaveTenant_Test()
        {
            // Arrange
            var tenant = new Tenant("TestTenant", "TestTenant");
            _tenantRepository.Insert(tenant);

            var tenant2 = new Tenant("TestTenant2", "TestTenant2");
            _tenantRepository.Insert(tenant2);

            using (var uow = _unitOfWorkManager.Begin())
            {
                _tenantFeatureRepository.Insert(new TenantFeatureSetting(tenant.Id, AppFeatures.SimpleBooleanFeature, "true"));
               _unitOfWorkManager.Current.SaveChanges();

                // Assert (before disable filter)
                _featureValueStore.GetValueOrNull(tenant.Id, AppFeatures.SimpleBooleanFeature).ShouldBe("true");
                _featureValueStore.GetValueOrNull(tenant2.Id, AppFeatures.SimpleBooleanFeature).ShouldBeNull();

                // Act (clear cache and disable filter)
                _cacheManager.GetTenantFeatureCache().Clear();

                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    // Assert (after disable filter)
                    _featureValueStore.GetValueOrNull(tenant.Id, AppFeatures.SimpleBooleanFeature).ShouldBe("true");
                    _featureValueStore.GetValueOrNull(tenant2.Id, AppFeatures.SimpleBooleanFeature).ShouldBeNull();
                }

                uow.Complete();
            }
        }

        [Fact]
        public async Task GetTenantFeatureCacheItemAsync_ShouldEnableFilterMayHaveTenant_Test()
        {
            // Arrange
            var tenant = new Tenant("TestTenant", "TestTenant");
            await _tenantRepository.InsertAsync(tenant);

            var tenant2 = new Tenant("TestTenant2", "TestTenant2");
            await _tenantRepository.InsertAsync(tenant2);

            using (var uow = _unitOfWorkManager.Begin())
            {
                await _tenantFeatureRepository.InsertAsync(new TenantFeatureSetting(tenant.Id, AppFeatures.SimpleBooleanFeature, "true"));
                await _unitOfWorkManager.Current.SaveChangesAsync();

                // Assert (before disable filter)
                (await _featureValueStore.GetValueOrNullAsync(tenant.Id, AppFeatures.SimpleBooleanFeature)).ShouldBe("true");
                (await _featureValueStore.GetValueOrNullAsync(tenant2.Id, AppFeatures.SimpleBooleanFeature)).ShouldBeNull();

                // Act (clear cache and disable filter)
                await _cacheManager.GetTenantFeatureCache().ClearAsync();

                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    // Assert (after disable filter)
                    (await _featureValueStore.GetValueOrNullAsync(tenant.Id, AppFeatures.SimpleBooleanFeature)).ShouldBe("true");
                    (await _featureValueStore.GetValueOrNullAsync(tenant2.Id, AppFeatures.SimpleBooleanFeature)).ShouldBeNull();
                }

                await uow.CompleteAsync();
            }
        }
    }
}
