using Abp.Entities.Core;

namespace Abp.Data.Repositories.Core
{
    /// <summary>
    /// Used to perform <see cref="Tenant"/> related database operations.
    /// </summary>
    public interface ITenantRepository : IRepository<Tenant, int>
    {

    }
}