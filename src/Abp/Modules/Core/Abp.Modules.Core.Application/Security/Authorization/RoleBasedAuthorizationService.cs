using System.Linq;
using Abp.Application.Authorization;
using Abp.Application.Authorization.Permissions;
using Abp.Dependency;
using Abp.Security.Roles.Management;
using Abp.Security.Users;
using Castle.Core.Logging;

namespace Abp.Security.Authorization
{
    //TODO: Make this class Singleton and create a Helper method to check permissions.
    public class RoleBasedAuthorizationService : IAuthorizationService, ITransientDependency
    {
        private readonly IPermissionDefinitionManager _permissionDefinitionManager;
        private readonly IRoleManager _roleManager;
        private readonly IUserRoleManager _userRoleManager;

        public ILogger Logger { get; set; }

        public RoleBasedAuthorizationService(
            IPermissionDefinitionManager permissionDefinitionManager,
            IRoleManager roleManager,
            IUserRoleManager userRoleManager)
        {
            _permissionDefinitionManager = permissionDefinitionManager;
            _roleManager = roleManager;
            _userRoleManager = userRoleManager;
            Logger = NullLogger.Instance;
        }

        public bool HasAnyOfPermissions(string[] permissionNames)
        {
            return permissionNames.Any(HasPermission);
        }

        public bool HasAllOfPermissions(string[] permissionNames)
        {
            return permissionNames.All(HasPermission);
        }

        public string[] GetAllPermissionNames()
        {
            return _permissionDefinitionManager.GetAllPermissions().Select(permission => permission.Name).ToArray();
        }

        public string[] GetGrantedPermissionNames()
        {
            return GetAllPermissionNames().Where(HasPermission).ToArray(); //TODO: Can be optimized?
        }

        private bool HasPermission(string permissionName)
        {
            var permission = _permissionDefinitionManager.GetPermissionOrNull(permissionName);
            if (permission == null)
            {
                Logger.Warn("Permission is not defined: " + permissionName);
                return false;
            }

            return HasPermission(permission);
        }

        private bool HasPermission(PermissionDefinition permissionDefinition)
        {
            if (!AbpUser.CurrentUserId.HasValue)
            {
                return false;
            }

            var roleNames = _userRoleManager.GetRolesOfUser(AbpUser.CurrentUserId.Value);
            var granted = permissionDefinition.IsGrantedByDefault;
            foreach (var roleName in roleNames)
            {
                var permissionSetting = _roleManager.GetPermissionOrNull(roleName, permissionDefinition.Name);
                if (permissionSetting == null)
                {
                    continue;
                }

                if (permissionSetting.IsGranted)
                {
                    return true; //Granted if any of role is granted
                }

                granted = false; //Denied for this role. Set false but check other roles
            }

            return granted;
        }
    }
}
