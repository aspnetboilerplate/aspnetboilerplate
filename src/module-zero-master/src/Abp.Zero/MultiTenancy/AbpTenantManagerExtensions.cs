using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Threading;

namespace Abp.MultiTenancy
{
    //TODO: Create other sync extension methods.
    public static class AbpTenantManagerExtensions
    {
        public static TTenant GetById<TTenant, TRole, TUser>(this AbpTenantManager<TTenant, TRole, TUser> tenantManager, int id)
            where TTenant : AbpTenant<TTenant, TUser>
            where TRole : AbpRole<TTenant, TUser>
            where TUser : AbpUser<TTenant, TUser>
        {
            return AsyncHelper.RunSync(() => tenantManager.GetByIdAsync(id));
        }
    }
}