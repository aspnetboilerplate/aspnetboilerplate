using Abp.Domain.Repositories.NHibernate;
using Abp.Modules.Core.Entities;
using Abp.Security.Tenants;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    /// <summary>
    /// Implements <see cref="ITenantRepository"/> for NHibernate.
    /// </summary>
    public class NhTenantRepository : NhRepositoryBase<Tenant>, ITenantRepository
    {

    }
}