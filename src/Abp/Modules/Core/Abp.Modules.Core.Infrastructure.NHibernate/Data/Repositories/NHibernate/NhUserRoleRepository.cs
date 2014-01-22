using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories.NHibernate;
using Abp.Roles;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    public class NhUserRoleRepository : NhRepositoryBase<UserRole>, IUserRoleRepository
    {
        public List<Role> GetRolesOfUser(int userId)
        {
            var query = from userRole in GetAll()
                        where userRole.User.Id == userId
                        select userRole.Role;
            return query.ToList();
        }
    }
}