using System.Collections.Generic;
using Abp.Domain.Repositories;

namespace Abp.Application.Authorization.Roles
{
    public interface IUserRoleRepository : IRepository<UserRole, long>
    {
        List<AbpRole> GetRolesOfUser(int userId);
    }
}