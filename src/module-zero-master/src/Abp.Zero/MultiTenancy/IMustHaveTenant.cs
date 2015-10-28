using Abp.Authorization.Users;
using Abp.Domain.Entities;

namespace Abp.MultiTenancy
{
    /// <summary>
    /// Implement this interface for an entity which must have Tenant.
    /// </summary>
    public interface IMustHaveTenant<TTenant, TUser> : IMustHaveTenant
        where TTenant : AbpTenant<TTenant, TUser>
        where TUser : AbpUser<TTenant, TUser>
    {
        /// <summary>
        /// Tenant.
        /// </summary>
        TTenant Tenant { get; set; }
    }
}
