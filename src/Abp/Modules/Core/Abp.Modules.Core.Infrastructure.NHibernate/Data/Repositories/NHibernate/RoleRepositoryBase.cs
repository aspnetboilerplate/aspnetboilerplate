using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories.NHibernate;
using Abp.Security.Roles;
using NHibernate.Linq;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    public abstract class RoleRepositoryBase<TRole> : NhRepositoryBase<TRole>, IRoleRepository<TRole> where TRole : AbpRole
    {
        public List<TRole> GetAllListWithPermissions()
        {
            return GetAll().Fetch(role => role.Permissions).ToList();
        }
    }
}