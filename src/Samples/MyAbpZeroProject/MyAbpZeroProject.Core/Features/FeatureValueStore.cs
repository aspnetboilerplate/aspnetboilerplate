using Abp.Application.Features;
using MyAbpZeroProject.Authorization.Roles;
using MyAbpZeroProject.MultiTenancy;
using MyAbpZeroProject.Users;

namespace MyAbpZeroProject.Features
{
    public class FeatureValueStore : AbpFeatureValueStore<Tenant, Role, User>
    {
        public FeatureValueStore(TenantManager tenantManager)
            : base(tenantManager)
        {
        }
    }
}