using System.Collections.Generic;
using Abp.Domain.Repositories;

namespace Abp.Roles
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        List<Role> GetRolesOfUser(int userId);
    }
}