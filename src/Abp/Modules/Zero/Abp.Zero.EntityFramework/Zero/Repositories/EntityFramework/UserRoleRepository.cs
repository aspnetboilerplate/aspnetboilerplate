using System.Collections.Generic;
using System.Linq;
using Abp.Application.Authorization.Roles;

namespace Abp.Zero.Repositories.EntityFramework
{
    public class UserRoleRepository : CoreModuleEfRepositoryBase<UserRole, long>, IUserRoleRepository
    {
        public List<AbpRole> GetRolesOfUser(int userId)
        {
            var query = from userRole in GetAll()
                        where userRole.User.Id == userId
                        select userRole.Role;
            return query.ToList();
        }
    }
}