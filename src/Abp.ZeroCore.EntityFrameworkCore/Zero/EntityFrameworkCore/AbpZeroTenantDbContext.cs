using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace Abp.Zero.EntityFrameworkCore
{
    [MultiTenancySide(MultiTenancySides.Tenant)]
    public abstract class AbpZeroTenantDbContext<TRole, TUser,TSelf> : AbpZeroCommonDbContext<TRole, TUser,TSelf>
        where TRole : AbpRole<TUser>
        where TUser : AbpUser<TUser>
        where TSelf: AbpZeroTenantDbContext<TRole, TUser, TSelf>
    {

        /// <summary>
        ///
        /// </summary>
        /// <param name="options"></param>
        protected AbpZeroTenantDbContext(DbContextOptions<TSelf> options)
            :base(options)
        {

        }
    }
}