using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Abp.Application.Authorization.Roles;

namespace Abp.Zero.Repositories.EntityFramework
{
    public class AbpRoleRepository : CoreModuleEfRepositoryBase<AbpRole>, IAbpRoleRepository
    {
        public List<AbpRole> GetAllListWithPermissions()
        {
            return GetAll().Include(role => role.Permissions).ToList();
        }
    }
}