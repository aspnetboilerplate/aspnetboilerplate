using System.Collections.Generic;
using Abp.Domain.Repositories;

namespace Abp.Security.Roles
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        List<Role> GetRolesOfUser(int userId);
    }
}