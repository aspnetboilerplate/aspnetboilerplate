using Abp.MultiTenancy;
using Abp.Zero.SampleApp.Editions;
using Abp.Zero.SampleApp.Roles;
using Abp.Zero.SampleApp.Users;

namespace Abp.Zero.SampleApp.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, Role, User>
    {
        public TenantManager(EditionManager editionManager) : 
            base(editionManager)
        {
        }
    }
}
