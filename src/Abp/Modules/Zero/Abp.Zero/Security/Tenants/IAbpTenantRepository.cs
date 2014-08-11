using Abp.Domain.Repositories;

namespace Abp.Security.Tenants
{
    /// <summary>
    /// Repository for Tenant entity.
    /// </summary>
    public interface IAbpTenantRepository : IRepository<AbpTenant>
    {

    }
}
