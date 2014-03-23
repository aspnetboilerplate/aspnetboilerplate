using Abp.Domain.Repositories.NHibernate;
using Abp.Security.Tenants;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    public class AbpTenantRepository : NhRepositoryBase<AbpTenant>, IAbpTenantRepository
    {

    }
}