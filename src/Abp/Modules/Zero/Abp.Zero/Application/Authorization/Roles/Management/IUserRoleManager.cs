using System.Collections.Generic;
using Abp.Dependency;

namespace Abp.Application.Authorization.Roles.Management
{
    public interface IUserRoleManager : ISingletonDependency
    {
        IReadOnlyList<string> GetRolesOfUser(long userId);
    }
}