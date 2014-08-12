using Abp.Domain.Repositories.NHibernate;
using Abp.MultiTenancy;

namespace Abp.Zero.Repositories.NHibernate
{
    public class AbpTenantRepository : NhRepositoryBase<AbpTenant>, IAbpTenantRepository
    {

    }
}