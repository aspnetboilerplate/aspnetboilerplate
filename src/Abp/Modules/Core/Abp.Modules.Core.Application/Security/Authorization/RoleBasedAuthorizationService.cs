using System.Linq;
using Abp.Application.Authorization;
using Abp.Application.Authorization.Permissions;
using Abp.Security.Roles;
using Abp.Security.Users;
using Castle.Core.Logging;

namespace Abp.Security.Authorization
{
    public class RoleBasedAuthorizationService : IAuthorizationService
    {
        private readonly IAbpUserRepository _userRepository;
        private readonly IUserRoleService _userRoleService;
        private readonly IPermissionManager _permissionManager;
        private readonly IRoleManager _roleManager;
        private readonly IUserRoleManager _userRoleManager;

        public ILogger Logger { get; set; }

        public RoleBasedAuthorizationService(
            IUserRoleService userRoleService, 
            IAbpUserRepository userRepository, 
            IPermissionManager permissionManager,
            IRoleManager roleManager, IUserRoleManager userRoleManager)
        {
            _userRoleService = userRoleService;
            _userRepository = userRepository;
            _permissionManager = permissionManager;
            _roleManager = roleManager;
            _userRoleManager = userRoleManager;
        }

        public bool HasAnyOfPermissions(string[] permissionNames)
        {
            return permissionNames.Any(HasPermission);
        }

        public bool HasAllOfPermissions(string[] permissionNames)
        {
            return permissionNames.All(HasPermission);
        }

        public bool HasPermission(string permissionName)
        {
            var permission = _permissionManager.GetPermissionOrNull(permissionName);
            if (permission == null)
            {
                Logger.Warn("Permission is not defined: " + permissionName);
                return true;
            }

            if (permission.IsGrantedByDefault)
            {
                return !IsDeniedForPermission(permission);
            }
            else
            {
                return IsGrantedForPermission(permission);                
            }
        }

        private bool IsGrantedForPermission(Permission permission)
        {
            var roleNames = _userRoleManager.GetRolesOfUser(AbpUser.CurrentUserId);
            foreach (var roleName in roleNames)
            {
                var permissionSetting = _roleManager.GetPermissionOrNull(roleName, permission.Name);
                if (permissionSetting != null && permissionSetting.IsGranted)
                {
                    return true;                    
                }
            }

            return false;
        }

        private bool IsDeniedForPermission(Permission permission)
        {
            var roleNames = _userRoleManager.GetRolesOfUser(AbpUser.CurrentUserId);
            foreach (var roleName in roleNames)
            {
                var permissionSetting = _roleManager.GetPermissionOrNull(roleName, permission.Name);
                if (permissionSetting != null && !permissionSetting.IsGranted)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
