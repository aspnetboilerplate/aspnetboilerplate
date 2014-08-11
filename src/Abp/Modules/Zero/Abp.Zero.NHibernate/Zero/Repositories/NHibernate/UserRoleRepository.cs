using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories.NHibernate;
using Abp.Security.Roles;

namespace Abp.Zero.Repositories.NHibernate
{
    public class UserRoleRepository : NhRepositoryBase<UserRole, long>, IUserRoleRepository
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