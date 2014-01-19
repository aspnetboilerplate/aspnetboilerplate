using Abp.Domain.Repositories;

namespace Abp.Tenants
{
    /// <summary>
    /// Used to perform <see cref="Tenant"/> related database operations.
    /// </summary>
    public interface ITenantRepository : IRepository<Tenant>
    {

    }
}