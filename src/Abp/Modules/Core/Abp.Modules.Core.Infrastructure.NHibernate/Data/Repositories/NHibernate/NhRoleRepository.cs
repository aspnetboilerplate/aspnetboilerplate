using Abp.Domain.Repositories.NHibernate;
using Abp.Security.Roles;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    public class NhRoleRepository : NhRepositoryBase<AbpRole>, IRoleRepository
    {
        
    }
}