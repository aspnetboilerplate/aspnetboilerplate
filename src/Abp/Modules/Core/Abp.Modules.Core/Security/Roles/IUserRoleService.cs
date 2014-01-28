using System.Collections.Generic;
using Abp.Domain.Services;
using Abp.Security.Users;

namespace Abp.Security.Roles
{
    public interface IUserRoleService : IDomainService
    {
        IReadOnlyCollection<Role> GetRolesOfUser(User user);
    }
}
