using Abp.Domain.Repositories;

namespace Abp.MultiTenancy
{
    /// <summary>
    /// Repository for Tenant entity.
    /// </summary>
    public interface IAbpTenantRepository : IRepository<AbpTenant>
    {

    }
}
