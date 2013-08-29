using Abp.Data.Repositories.Core;
using Abp.Entities.Core;

namespace Abp.Data.Repositories.NHibernate.Core
{
    /// <summary>
    /// Implements <see cref="ITenantRepository"/> for NHibernate.
    /// </summary>
    public class NhTenantRepository : NhRepositoryBase<Tenant, int>, ITenantRepository
    {

    }
}