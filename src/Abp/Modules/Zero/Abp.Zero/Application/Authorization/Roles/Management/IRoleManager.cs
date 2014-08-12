using Abp.Application.Authorization.Permissions;
using Abp.Dependency;

namespace Abp.Application.Authorization.Roles.Management
{
    public interface IRoleManager : ISingletonDependency
    {
        Permission GetPermissionOrNull(string roleName, string permissionName);
    }
}