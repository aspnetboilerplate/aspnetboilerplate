using Abp.MultiTenancy;
using MyAbpZeroProject.Authorization.Roles;
using MyAbpZeroProject.Editions;
using MyAbpZeroProject.Users;

namespace MyAbpZeroProject.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, Role, User>
    {
        public TenantManager(EditionManager editionManager)
            : base(editionManager)
        {

        }
    }
}