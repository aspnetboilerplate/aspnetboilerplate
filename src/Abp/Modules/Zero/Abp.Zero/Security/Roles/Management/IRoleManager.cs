using Abp.Dependency;
using Abp.Security.Permissions;

namespace Abp.Security.Roles.Management
{
    public interface IRoleManager : ISingletonDependency
    {
        Permission GetPermissionOrNull(string roleName, string permissionName);
    }
}