using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories.EntityFramework;
using Abp.Security.Roles;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    public class UserRoleRepository : EfRepositoryBase<UserRole, long>, IUserRoleRepository
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