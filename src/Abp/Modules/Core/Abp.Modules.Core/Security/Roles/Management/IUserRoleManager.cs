using System.Collections.Generic;
using Abp.Dependency;

namespace Abp.Security.Roles.Management
{
    public interface IUserRoleManager : ISingletonDependency
    {
        IReadOnlyList<string> GetRolesOfUser(long userId);
    }
}