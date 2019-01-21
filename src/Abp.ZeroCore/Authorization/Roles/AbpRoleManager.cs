using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Abp.Authorization.Roles
{
    public class AbpRoleManager<TRole, TUser> : RoleManager<TRole>, IDomainService
        where TRole : AbpRole<TUser>, new()
        where TUser : AbpUser<TUser>
    {
        public ILocalizationManager LocalizationManager { get; set; }

        protected string LocalizationSourceName { get; set; }

        public IAbpSession AbpSession { get; set; }

        public IRoleManagementConfig RoleManagementConfig { get; }

        public FeatureDependencyContext FeatureDependencyContext { get; set; }

        private IRolePermissionStore<TRole> RolePermissionStore
        {
            get
            {
                if (!(Store is IRolePermissionStore<TRole>))
                {
                    throw new AbpException("Store is not IRolePermissionStore");
                }

                return Store as IRolePermissionStore<TRole>;
            }
        }

        protected AbpRoleStore<TRole, TUser> AbpStore { get; }

        private readonly IPermissionManager _permissionManager;
        private readonly ICacheManager _cacheManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public AbpRoleManager(
            AbpRoleStore<TRole, TUser> store,
            IEnumerable<IRoleValidator<TRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<AbpRoleManager<TRole, TUser>> logger,
            IPermissionManager permissionManager,
            ICacheManager cacheManager, 
            IUnitOfWorkManager unitOfWorkManager,
            IRoleManagementConfig roleManagementConfig)
            : base(
                  store,
                  roleValidators,
                  keyNormalizer,
                  errors,
                  logger)
        {
            _permissionManager = permissionManager;
            _cacheManager = cacheManager;
            _unitOfWorkManager = unitOfWorkManager;

            RoleManagementConfig = roleManagementConfig;
            AbpStore = store;
            AbpSession = NullAbpSession.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
            LocalizationSourceName = AbpZeroConsts.LocalizationSourceName;
        }

        /// <summary>
        /// Checks if a role is granted for a permission.
        /// </summary>
        /// <param name="roleName">The role's name to check it's permission</param>
        /// <param name="permissionName">Name of the permission</param>
        /// <returns>True, if the role has the permission</returns>
        public virtual async Task<bool> IsGrantedAsync(string roleName, string permissionName)
        {
            return await IsGrantedAsync((await GetRoleByNameAsync(roleName)).Id, _permissionManager.GetPermission(permissionName));
        }

        /// <summary>
        /// Checks if a role has a permission.
        /// </summary>
        /// <param name="roleId">The role's id to check it's permission</param>
        /// <param name="permissionName">Name of the permission</param>
        /// <returns>True, if the role has the permission</returns>
        public virtual async Task<bool> IsGrantedAsync(int roleId, string permissionName)
        {
            return await IsGrantedAsync(roleId, _permissionManager.GetPermission(permissionName));
        }

        /// <summary>
        /// Checks if a role is granted for a permission.
        /// </summary>
        /// <param name="role">The role</param>
        /// <param name="permission">The permission</param>
        /// <returns>True, if the role has the permission</returns>
        public Task<bool> IsGrantedAsync(TRole role, Permission permission)
        {
            return IsGrantedAsync(role.Id, permission);
        }

        /// <summary>
        /// Checks if a role is granted for a permission.
        /// </summary>
        /// <param name="roleId">role id</param>
        /// <param name="permission">The permission</param>
        /// <returns>True, if the role has the permission</returns>
        public virtual async Task<bool> IsGrantedAsync(int roleId, Permission permission)
        {
            //Get cached role permissions
            var cacheItem = await GetRolePermissionCacheItemAsync(roleId);

            //Check the permission
            return cacheItem.GrantedPermissions.Contains(permission.Name);
        }

        /// <summary>
        /// Gets granted permission names for a role.
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(int roleId)
        {
            return await GetGrantedPermissionsAsync(await GetRoleByIdAsync(roleId));
        }

        /// <summary>
        /// Gets granted permission names for a role.
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(string roleName)
        {
            return await GetGrantedPermissionsAsync(await GetRoleByNameAsync(roleName));
        }

        /// <summary>
        /// Gets granted permissions for a role.
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(TRole role)
        {
            var permissionList = new List<Permission>();

            foreach (var permission in _permissionManager.GetAllPermissions())
            {
                if (await IsGrantedAsync(role.Id, permission))
                {
                    permissionList.Add(permission);
                }
            }

            return permissionList;
        }

        /// <summary>
        /// Sets all granted permissions of a role at once.
        /// Prohibits all other permissions.
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <param name="permissions">Permissions</param>
        public virtual async Task SetGrantedPermissionsAsync(int roleId, IEnumerable<Permission> permissions)
        {
            await SetGrantedPermissionsAsync(await GetRoleByIdAsync(roleId), permissions);
        }

        /// <summary>
        /// Sets all granted permissions of a role at once.
        /// Prohibits all other permissions.
        /// </summary>
        /// <param name="role">The role</param>
        /// <param name="permissions">Permissions</param>
        public virtual async Task SetGrantedPermissionsAsync(TRole role, IEnumerable<Permission> permissions)
        {
            var oldPermissions = await GetGrantedPermissionsAsync(role);
            var newPermissions = permissions.ToArray();

            foreach (var permission in oldPermissions.Where(p => !newPermissions.Contains(p, PermissionEqualityComparer.Instance)))
            {
                await ProhibitPermissionAsync(role, permission);
            }

            foreach (var permission in newPermissions.Where(p => !oldPermissions.Contains(p, PermissionEqualityComparer.Instance)))
            {
                await GrantPermissionAsync(role, permission);
            }
        }

        /// <summary>
        /// Grants a permission for a role.
        /// </summary>
        /// <param name="role">Role</param>
        /// <param name="permission">Permission</param>
        public async Task GrantPermissionAsync(TRole role, Permission permission)
        {
            if (await IsGrantedAsync(role.Id, permission))
            {
                return;
            }

            await RolePermissionStore.RemovePermissionAsync(role, new PermissionGrantInfo(permission.Name, false));
            await RolePermissionStore.AddPermissionAsync(role, new PermissionGrantInfo(permission.Name, true));
        }

        /// <summary>
        /// Prohibits a permission for a role.
        /// </summary>
        /// <param name="role">Role</param>
        /// <param name="permission">Permission</param>
        public async Task ProhibitPermissionAsync(TRole role, Permission permission)
        {
            if (!await IsGrantedAsync(role.Id, permission))
            {
                return;
            }

            await RolePermissionStore.RemovePermissionAsync(role, new PermissionGrantInfo(permission.Name, true));
            await RolePermissionStore.AddPermissionAsync(role, new PermissionGrantInfo(permission.Name, false));
        }

        /// <summary>
        /// Prohibits all permissions for a role.
        /// </summary>
        /// <param name="role">Role</param>
        public async Task ProhibitAllPermissionsAsync(TRole role)
        {
            foreach (var permission in _permissionManager.GetAllPermissions())
            {
                await ProhibitPermissionAsync(role, permission);
            }
        }

        /// <summary>
        /// Resets all permission settings for a role.
        /// It removes all permission settings for the role.
        /// Role will have permissions those have <see cref="Permission.IsGrantedByDefault"/> set to true.
        /// </summary>
        /// <param name="role">Role</param>
        public async Task ResetAllPermissionsAsync(TRole role)
        {
            await RolePermissionStore.RemoveAllPermissionSettingsAsync(role);
        }

        /// <summary>
        /// Creates a role.
        /// </summary>
        /// <param name="role">Role</param>
        public override async Task<IdentityResult> CreateAsync(TRole role)
        {
            var result = await CheckDuplicateRoleNameAsync(role.Id, role.Name, role.DisplayName);
            if (!result.Succeeded)
            {
                return result;
            }

            var tenantId = GetCurrentTenantId();
            if (tenantId.HasValue && !role.TenantId.HasValue)
            {
                role.TenantId = tenantId.Value;
            }

            return await base.CreateAsync(role);
        }

        public override async Task<IdentityResult> UpdateAsync(TRole role)
        {
            var result = await CheckDuplicateRoleNameAsync(role.Id, role.Name, role.DisplayName);
            if (!result.Succeeded)
            {
                return result;
            }

            return await base.UpdateAsync(role);
        }

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="role">Role</param>
        public override async Task<IdentityResult> DeleteAsync(TRole role)
        {
            if (role.IsStatic)
            {
                throw new UserFriendlyException(string.Format(L("CanNotDeleteStaticRole"), role.Name));
            }

            return await base.DeleteAsync(role);
        }

        /// <summary>
        /// Gets a role by given id.
        /// Throws exception if no role with given id.
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <returns>Role</returns>
        /// <exception cref="AbpException">Throws exception if no role with given id</exception>
        public virtual async Task<TRole> GetRoleByIdAsync(int roleId)
        {
            var role = await FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new AbpException("There is no role with id: " + roleId);
            }

            return role;
        }

        /// <summary>
        /// Gets a role by given name.
        /// Throws exception if no role with given roleName.
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Role</returns>
        /// <exception cref="AbpException">Throws exception if no role with given roleName</exception>
        public virtual async Task<TRole> GetRoleByNameAsync(string roleName)
        {
            var role = await FindByNameAsync(roleName);
            if (role == null)
            {
                throw new AbpException("There is no role with name: " + roleName);
            }

            return role;
        }

        public async Task GrantAllPermissionsAsync(TRole role)
        {
            FeatureDependencyContext.TenantId = role.TenantId;

            var permissions = _permissionManager.GetAllPermissions(role.GetMultiTenancySide())
                                                .Where(permission =>
                                                    permission.FeatureDependency == null ||
                                                    permission.FeatureDependency.IsSatisfied(FeatureDependencyContext)
                                                );

            await SetGrantedPermissionsAsync(role, permissions);
        }

        [UnitOfWork]
        public virtual async Task<IdentityResult> CreateStaticRoles(int tenantId)
        {
            var staticRoleDefinitions = RoleManagementConfig.StaticRoles.Where(sr => sr.Side == MultiTenancySides.Tenant);

            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                foreach (var staticRoleDefinition in staticRoleDefinitions)
                {
                    var role = new TRole
                    {
                        TenantId = tenantId,
                        Name = staticRoleDefinition.RoleName,
                        DisplayName = staticRoleDefinition.RoleName,
                        IsStatic = true
                    };

                    var identityResult = await CreateAsync(role);
                    if (!identityResult.Succeeded)
                    {
                        return identityResult;
                    }
                }
            }

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> CheckDuplicateRoleNameAsync(int? expectedRoleId, string name, string displayName)
        {
            var role = await FindByNameAsync(name);
            if (role != null && role.Id != expectedRoleId)
            {
                throw new UserFriendlyException(string.Format(L("RoleNameIsAlreadyTaken"), name));
            }

            role = await FindByDisplayNameAsync(displayName);
            if (role != null && role.Id != expectedRoleId)
            {
                throw new UserFriendlyException(string.Format(L("RoleDisplayNameIsAlreadyTaken"), displayName));
            }

            return IdentityResult.Success;
        }

        private Task<TRole> FindByDisplayNameAsync(string displayName)
        {
            return AbpStore.FindByDisplayNameAsync(displayName);
        }

        private async Task<RolePermissionCacheItem> GetRolePermissionCacheItemAsync(int roleId)
        {
            var cacheKey = roleId + "@" + (GetCurrentTenantId() ?? 0);
            return await _cacheManager.GetRolePermissionCache().GetAsync(cacheKey, async () =>
            {
                var newCacheItem = new RolePermissionCacheItem(roleId);

                var role = await Store.FindByIdAsync(roleId.ToString(), CancellationToken);
                if (role == null)
                {
                    throw new AbpException("There is no role with given id: " + roleId);
                }

                var staticRoleDefinition = RoleManagementConfig.StaticRoles.FirstOrDefault(r =>
                    r.RoleName == role.Name && r.Side == role.GetMultiTenancySide());

                if (staticRoleDefinition != null)
                {
                    foreach (var permission in _permissionManager.GetAllPermissions())
                    {
                        if (staticRoleDefinition.IsGrantedByDefault(permission))
                        {
                            newCacheItem.GrantedPermissions.Add(permission.Name);
                        }
                    }
                }

                foreach (var permissionInfo in await RolePermissionStore.GetPermissionsAsync(roleId))
                {
                    if (permissionInfo.IsGranted)
                    {
                        newCacheItem.GrantedPermissions.AddIfNotContains(permissionInfo.Name);
                    }
                    else
                    {
                        newCacheItem.GrantedPermissions.Remove(permissionInfo.Name);
                    }
                }

                return newCacheItem;
            });
        }

        protected virtual string L(string name)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name);
        }

        protected virtual string L(string name, CultureInfo cultureInfo)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name, cultureInfo);
        }

        private int? GetCurrentTenantId()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _unitOfWorkManager.Current.GetTenantId();
            }

            return AbpSession.TenantId;
        }
    }
}
