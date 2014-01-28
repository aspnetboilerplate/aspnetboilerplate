using System.Linq;
using Abp.Application.Authorization;
using Abp.Application.Authorization.Permissions;
using Abp.Domain.Uow;
using Abp.Security.Roles;
using Abp.Security.Users;
using Castle.Core.Logging;

namespace Abp.Security.Authorization
{
    public class RoleBasedAuthorizationService : IAuthorizationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleService _userRoleService;
        private readonly IPermissionManager _permissionManager;

        public ILogger Logger { get; set; }

        public RoleBasedAuthorizationService(IUserRoleService userRoleService, IUserRepository userRepository, IPermissionManager permissionManager)
        {
            _userRoleService = userRoleService;
            _userRepository = userRepository;
            _permissionManager = permissionManager;
        }

        public bool HasAnyOfPermissions(string[] permissionNames)
        {
            return permissionNames.Any(HasPermission);
        }

        public bool HasAllOfPermissions(string[] permissionNames)
        {
            return permissionNames.All(HasPermission);
        }

        [UnitOfWork]
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
            var currentUser = _userRepository.Load(User.CurrentUserId);
            var roles = _userRoleService.GetRolesOfUser(currentUser);
            foreach (var role in roles)
            {
                foreach (var rolePermission in role.Permissions) //TODO: Permissions are not laoded!
                {
                    if (permission.Name == rolePermission.PermissionName && rolePermission.IsGranted)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsDeniedForPermission(Permission permission)
        {
            var currentUser = _userRepository.Load(User.CurrentUserId);
            var roles = _userRoleService.GetRolesOfUser(currentUser);
            foreach (var role in roles)
            {
                foreach (var rolePermission in role.Permissions)
                {
                    if (permission.Name == rolePermission.PermissionName && !rolePermission.IsGranted)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
