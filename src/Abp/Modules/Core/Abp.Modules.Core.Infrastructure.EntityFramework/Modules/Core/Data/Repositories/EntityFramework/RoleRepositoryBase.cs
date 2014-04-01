using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Abp.Domain.Repositories.EntityFramework;
using Abp.Security.Roles;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    public abstract class RoleRepositoryBase<TRole> : EfRepositoryBase<TRole>, IRoleRepository<TRole> where TRole : AbpRole
    {
        public List<TRole> GetAllListWithPermissions()
        {
            return GetAll().Include(role => role.Permissions).ToList();
        }
    }
}