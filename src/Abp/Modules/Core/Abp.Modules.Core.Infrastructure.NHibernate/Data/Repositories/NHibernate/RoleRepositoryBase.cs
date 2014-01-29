using Abp.Domain.Repositories.NHibernate;
using Abp.Security.Roles;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    public abstract class RoleRepositoryBase<TRole> : NhRepositoryBase<TRole>, IRoleRepository<TRole> where TRole : AbpRole
    {

    }
}