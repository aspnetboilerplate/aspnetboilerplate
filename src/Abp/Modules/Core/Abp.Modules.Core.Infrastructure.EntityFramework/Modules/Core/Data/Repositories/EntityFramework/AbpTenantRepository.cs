using Abp.Domain.Repositories.EntityFramework;
using Abp.Security.Tenants;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    public class AbpTenantRepository : EfRepositoryBase<AbpTenant>, IAbpTenantRepository
    {

    }
}