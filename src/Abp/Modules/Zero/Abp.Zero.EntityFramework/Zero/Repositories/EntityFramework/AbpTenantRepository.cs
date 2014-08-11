using Abp.Security.Tenants;

namespace Abp.Zero.Repositories.EntityFramework
{
    public class AbpTenantRepository : CoreModuleEfRepositoryBase<AbpTenant>, IAbpTenantRepository
    {

    }
}