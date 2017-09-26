using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Abp.Zero.SampleApp.Editions;
using Abp.Zero.SampleApp.Users;

namespace Abp.Zero.SampleApp.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public TenantManager(
            IRepository<Tenant> tenantRepository,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            EditionManager editionManager,
            IAbpZeroFeatureValueStore featureValueStore) :
            base(tenantRepository, tenantFeatureRepository, editionManager, featureValueStore)
        {
        }
    }
}
