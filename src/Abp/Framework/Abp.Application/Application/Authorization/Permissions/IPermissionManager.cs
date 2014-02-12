using Abp.Dependency;

namespace Abp.Application.Authorization.Permissions
{
    public interface IPermissionManager : ISingletonDependency
    {
        Permission GetPermissionOrNull(string permissionName);
    }
}
