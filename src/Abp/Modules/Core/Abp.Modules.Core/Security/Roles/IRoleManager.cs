using System.Collections.Generic;
using Abp.Dependency;

namespace Abp.Security.Roles
{
    public interface IRoleManager : ISingletonDependency
    {
        RolePermission GetPermissionOrNull(string roleName, string permissionName);
    }
}