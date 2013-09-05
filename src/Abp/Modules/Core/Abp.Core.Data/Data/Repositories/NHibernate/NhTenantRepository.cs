using Abp.Data.Repositories.NHibernate;
using Abp.Modules.Core.Entities.Core;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    /// <summary>
    /// Implements <see cref="ITenantRepository"/> for NHibernate.
    /// </summary>
    public class NhTenantRepository : NhRepositoryBase<Tenant, int>, ITenantRepository
    {

    }
}