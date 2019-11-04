using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Microsoft.AspNet.Identity;

namespace Abp.Authorization.Roles
{
    /// <summary>
    /// Implements 'Role Store' of ASP.NET Identity Framework.
    /// </summary>
    public abstract class AbpRoleStore<TRole, TUser> :
        IQueryableRoleStore<TRole, int>,
        IRolePermissionStore<TRole>,
        ITransientDependency

        where TRole : AbpRole<TUser>
        where TUser : AbpUser<TUser>
    {
        private readonly IRepository<TRole> _roleRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<RolePermissionSetting, long> _rolePermissionSettingRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpRoleStore(
            IRepository<TRole> roleRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<RolePermissionSetting, long> rolePermissionSettingRepository)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _rolePermissionSettingRepository = rolePermissionSettingRepository;
        }

        public virtual IQueryable<TRole> Roles
        {
            get { return _roleRepository.GetAll(); }
        }

        public virtual async Task CreateAsync(TRole role)
        {
            await _roleRepository.InsertAsync(role);
        }

        public virtual async Task UpdateAsync(TRole role)
        {
            await _roleRepository.UpdateAsync(role);
        }

        public virtual async Task DeleteAsync(TRole role)
        {
            await _userRoleRepository.DeleteAsync(ur => ur.RoleId == role.Id);
            await _roleRepository.DeleteAsync(role);
        }

        public virtual async Task<TRole> FindByIdAsync(int roleId)
        {
            return await _roleRepository.FirstOrDefaultAsync(roleId);
        }

        public virtual TRole FindById(int roleId)
        {
            return _roleRepository.FirstOrDefault(roleId);
        }

        public virtual async Task<TRole> FindByNameAsync(string roleName)
        {
            var normalizedName = NormalizeKey(roleName);

            return await _roleRepository.FirstOrDefaultAsync(
                role => role.NormalizedName == normalizedName
                );
        }

        public virtual TRole FindByName(string roleName)
        {
            var normalizedName = NormalizeKey(roleName);

            return _roleRepository.FirstOrDefault(
                role => role.NormalizedName == normalizedName
                );
        }

        public virtual async Task<TRole> FindByDisplayNameAsync(string displayName)
        {
            return await _roleRepository.FirstOrDefaultAsync(
                role => role.DisplayName == displayName
                );
        }

        /// <inheritdoc/>
        public virtual async Task AddPermissionAsync(TRole role, PermissionGrantInfo permissionGrant)
        {
            if (await HasPermissionAsync(role.Id, permissionGrant))
            {
                return;
            }

            await _rolePermissionSettingRepository.InsertAsync(
                new RolePermissionSetting
                {
                    TenantId = role.TenantId,
                    RoleId = role.Id,
                    Name = permissionGrant.Name,
                    IsGranted = permissionGrant.IsGranted
                });
        }

        /// <inheritdoc/>
        public virtual async Task RemovePermissionAsync(TRole role, PermissionGrantInfo permissionGrant)
        {
            await _rolePermissionSettingRepository.DeleteAsync(
                permissionSetting => permissionSetting.RoleId == role.Id &&
                                     permissionSetting.Name == permissionGrant.Name &&
                                     permissionSetting.IsGranted == permissionGrant.IsGranted
                );
        }

        /// <inheritdoc/>
        public virtual Task<IList<PermissionGrantInfo>> GetPermissionsAsync(TRole role)
        {
            return GetPermissionsAsync(role.Id);
        }

        /// <inheritdoc/>
        public virtual IList<PermissionGrantInfo> GetPermissions(TRole role)
        {
            return GetPermissions(role.Id);
        }

        public async Task<IList<PermissionGrantInfo>> GetPermissionsAsync(int roleId)
        {
            return (await _rolePermissionSettingRepository.GetAllListAsync(p => p.RoleId == roleId))
                .Select(p => new PermissionGrantInfo(p.Name, p.IsGranted))
                .ToList();
        }

        public IList<PermissionGrantInfo> GetPermissions(int roleId)
        {
            return (_rolePermissionSettingRepository.GetAllList(p => p.RoleId == roleId))
                .Select(p => new PermissionGrantInfo(p.Name, p.IsGranted))
                .ToList();
        }

        /// <inheritdoc/>
        public virtual async Task<bool> HasPermissionAsync(int roleId, PermissionGrantInfo permissionGrant)
        {
            return await _rolePermissionSettingRepository.FirstOrDefaultAsync(
                p => p.RoleId == roleId &&
                     p.Name == permissionGrant.Name &&
                     p.IsGranted == permissionGrant.IsGranted
                ) != null;
        }

        /// <inheritdoc/>
        public virtual async Task RemoveAllPermissionSettingsAsync(TRole role)
        {
            await _rolePermissionSettingRepository.DeleteAsync(s => s.RoleId == role.Id);
        }

        protected virtual string NormalizeKey(string key)
        {
            return key.ToUpperInvariant();
        }

        public virtual void Dispose()
        {
            //No need to dispose since using IOC.
        }
    }
}
