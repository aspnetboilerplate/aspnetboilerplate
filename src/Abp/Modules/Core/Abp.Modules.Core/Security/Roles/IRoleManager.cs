using Abp.Dependency;

namespace Abp.Security.Roles
{
    public interface IRoleManager : ISingletonDependency
    {
        Permission GetPermissionOrNull(string roleName, string permissionName);
    }
}