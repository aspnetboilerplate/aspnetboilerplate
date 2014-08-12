using Abp.MultiTenancy;

namespace Abp.Zero.Repositories.EntityFramework
{
    public class AbpTenantRepository : CoreModuleEfRepositoryBase<AbpTenant>, IAbpTenantRepository
    {

    }
}