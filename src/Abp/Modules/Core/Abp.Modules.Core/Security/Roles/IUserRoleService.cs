using System.Collections.Generic;
using Abp.Domain.Services;
using Abp.Security.Users;

namespace Abp.Security.Roles
{
    public interface IUserRoleService : IDomainService
    {
        IReadOnlyCollection<AbpRole> GetRolesOfUser(AbpUser user);
    }
}
