using Abp.Domain.Repositories.NHibernate;
using Abp.Security.Tenants;

namespace Abp.Zero.Repositories.NHibernate
{
    public class AbpTenantRepository : NhRepositoryBase<AbpTenant>, IAbpTenantRepository
    {

    }
}