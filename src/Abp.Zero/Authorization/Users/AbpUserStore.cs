using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Authorization.Roles;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq;
using Abp.Organizations;
using Microsoft.AspNet.Identity;

namespace Abp.Authorization.Users
{
    /// <summary>
    /// Implements 'User Store' of ASP.NET Identity Framework.
    /// </summary>
    public abstract class AbpUserStore<TRole, TUser> :
        IUserStore<TUser, long>,
        IUserPasswordStore<TUser, long>,
        IUserEmailStore<TUser, long>,
        IUserLoginStore<TUser, long>,
        IUserRoleStore<TUser, long>,
        IQueryableUserStore<TUser, long>,
        IUserLockoutStore<TUser, long>,
        IUserPermissionStore<TUser>,
        IUserPhoneNumberStore<TUser, long>,
        IUserClaimStore<TUser, long>,
        IUserSecurityStampStore<TUser, long>,
        IUserTwoFactorStore<TUser, long>,

        ITransientDependency

        where TRole : AbpRole<TUser>
        where TUser : AbpUser<TUser>
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        private readonly IRepository<TUser, long> _userRepository;
        private readonly IRepository<UserLogin, long> _userLoginRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<UserClaim, long> _userClaimRepository;
        private readonly IRepository<TRole> _roleRepository;
        private readonly IRepository<UserPermissionSetting, long> _userPermissionSettingRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly IRepository<OrganizationUnitRole, long> _organizationUnitRoleRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpUserStore(
            IRepository<TUser, long> userRepository,
            IRepository<UserLogin, long> userLoginRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<TRole> roleRepository,
            IRepository<UserPermissionSetting, long> userPermissionSettingRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserClaim, long> userClaimRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository)
        {
            _userRepository = userRepository;
            _userLoginRepository = userLoginRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _userClaimRepository = userClaimRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _organizationUnitRoleRepository = organizationUnitRoleRepository;
            _userPermissionSettingRepository = userPermissionSettingRepository;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        #region IQueryableUserStore

        public virtual IQueryable<TUser> Users => _userRepository.GetAll();

        #endregion

        #region IUserStore

        public virtual async Task CreateAsync(TUser user)
        {
            await _userRepository.InsertAsync(user);
        }

        public virtual async Task UpdateAsync(TUser user)
        {
            await _userRepository.UpdateAsync(user);
        }

        public virtual async Task DeleteAsync(TUser user)
        {
            await _userRepository.DeleteAsync(user.Id);
        }

        public virtual async Task<TUser> FindByIdAsync(long userId)
        {
            return await _userRepository.FirstOrDefaultAsync(userId);
        }

        public virtual TUser FindById(long userId)
        {
            return _userRepository.FirstOrDefault(userId);
        }

        public virtual async Task<TUser> FindByNameAsync(string userName)
        {
            var normalizedUsername = NormalizeKey(userName);

            return await _userRepository.FirstOrDefaultAsync(
                user => user.NormalizedUserName == normalizedUsername
            );
        }

        public virtual TUser FindByName(string userName)
        {
            var normalizedUsername = NormalizeKey(userName);

            return _userRepository.FirstOrDefault(
                user => user.NormalizedUserName == normalizedUsername
            );
        }

        public virtual async Task<TUser> FindByEmailAsync(string email)
        {
            var normalizedEmail = NormalizeKey(email);

            return await _userRepository.FirstOrDefaultAsync(
                user => user.NormalizedEmailAddress == normalizedEmail
            );
        }

        /// <summary>
        /// Tries to find a user with user name or email address in current tenant.
        /// </summary>
        /// <param name="userNameOrEmailAddress">User name or email address</param>
        /// <returns>User or null</returns>
        public virtual async Task<TUser> FindByNameOrEmailAsync(string userNameOrEmailAddress)
        {
            var normalizedUserNameOrEmailAddress = NormalizeKey(userNameOrEmailAddress);

            return await _userRepository.FirstOrDefaultAsync(
                user => (user.NormalizedUserName == normalizedUserNameOrEmailAddress || user.NormalizedEmailAddress == normalizedUserNameOrEmailAddress)
            );
        }

        /// <summary>
        /// Tries to find a user with user name or email address in given tenant.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="userNameOrEmailAddress">User name or email address</param>
        /// <returns>User or null</returns>
        [UnitOfWork]
        public virtual async Task<TUser> FindByNameOrEmailAsync(int? tenantId, string userNameOrEmailAddress)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return await FindByNameOrEmailAsync(userNameOrEmailAddress);
            }
        }

        #endregion

        #region IUserPasswordStore

        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            user.Password = passwordHash;
            return Task.FromResult(0);
        }

        public virtual Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.FromResult(user.Password);
        }

        public virtual Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.Password));
        }

        #endregion

        #region IUserEmailStore

        public virtual Task SetEmailAsync(TUser user, string email)
        {
            user.EmailAddress = email;
            return Task.FromResult(0);
        }

        public virtual Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(user.EmailAddress);
        }

        public virtual Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.IsEmailConfirmed);
        }

        public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            user.IsEmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        #endregion

        #region IUserLoginStore

        public virtual async Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            await _userLoginRepository.InsertAsync(
                new UserLogin
                {
                    TenantId = user.TenantId,
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey,
                    UserId = user.Id
                });
        }

        public virtual async Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            await _userLoginRepository.DeleteAsync(
                ul => ul.UserId == user.Id &&
                      ul.LoginProvider == login.LoginProvider &&
                      ul.ProviderKey == login.ProviderKey
            );
        }

        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            return (await _userLoginRepository.GetAllListAsync(ul => ul.UserId == user.Id))
                .Select(ul => new UserLoginInfo(ul.LoginProvider, ul.ProviderKey))
                .ToList();
        }

        public virtual async Task<TUser> FindAsync(UserLoginInfo login)
        {
            var userLogin = await _userLoginRepository.FirstOrDefaultAsync(
                ul => ul.LoginProvider == login.LoginProvider && ul.ProviderKey == login.ProviderKey
            );

            if (userLogin == null)
            {
                return null;
            }

            return await _userRepository.FirstOrDefaultAsync(u => u.Id == userLogin.UserId);
        }

        [UnitOfWork]
        public virtual Task<List<TUser>> FindAllAsync(UserLoginInfo login)
        {
            var query = from userLogin in _userLoginRepository.GetAll()
                        join user in _userRepository.GetAll() on userLogin.UserId equals user.Id
                        where userLogin.LoginProvider == login.LoginProvider && userLogin.ProviderKey == login.ProviderKey
                        select user;

            return Task.FromResult(query.ToList());
        }

        public virtual Task<TUser> FindAsync(int? tenantId, UserLoginInfo login)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var query = from userLogin in _userLoginRepository.GetAll()
                            join user in _userRepository.GetAll() on userLogin.UserId equals user.Id
                            where userLogin.LoginProvider == login.LoginProvider && userLogin.ProviderKey == login.ProviderKey
                            select user;

                return Task.FromResult(query.FirstOrDefault());
            }
        }

        #endregion

        #region IUserRoleStore

        public virtual async Task AddToRoleAsync(TUser user, string roleName)
        {
            var role = await GetRoleByNameAsync(roleName);
            await _userRoleRepository.InsertAsync(new UserRole(user.TenantId, user.Id, role.Id));
        }

        public virtual async Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            var role = await GetRoleByNameAsync(roleName);
            var userRole = await _userRoleRepository.FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);
            if (userRole == null)
            {
                return;
            }

            await _userRoleRepository.DeleteAsync(userRole);
        }

        [UnitOfWork]
        public virtual async Task<IList<string>> GetRolesAsync(TUser user)
        {
            var userRoles = await AsyncQueryableExecuter.ToListAsync(from userRole in _userRoleRepository.GetAll()
                                                                     join role in _roleRepository.GetAll() on userRole.RoleId equals role.Id
                                                                     where userRole.UserId == user.Id
                                                                     select role.Name);

            var userOrganizationUnitRoles = await AsyncQueryableExecuter.ToListAsync(
                from userOu in _userOrganizationUnitRepository.GetAll()
                join roleOu in _organizationUnitRoleRepository.GetAll() on userOu.OrganizationUnitId equals roleOu
                    .OrganizationUnitId
                join userOuRoles in _roleRepository.GetAll() on roleOu.RoleId equals userOuRoles.Id
                where userOu.UserId == user.Id
                select userOuRoles.Name);

            return userRoles.Union(userOrganizationUnitRoles).ToList();
        }

        [UnitOfWork]
        public virtual IList<string> GetRoles(TUser user) => GetRoles(user.Id);

        [UnitOfWork]
        public virtual IList<string> GetRoles(long userId)
        {
            var userRoles = AsyncQueryableExecuter.ToList(from userRole in _userRoleRepository.GetAll()
                                                          join role in _roleRepository.GetAll() on userRole.RoleId equals role.Id
                                                          where userRole.UserId == userId
                                                          select role.Name);

            var userOrganizationUnitRoles = AsyncQueryableExecuter.ToList(
                from userOu in _userOrganizationUnitRepository.GetAll()
                join roleOu in _organizationUnitRoleRepository.GetAll() on userOu.OrganizationUnitId equals roleOu
                    .OrganizationUnitId
                join userOuRoles in _roleRepository.GetAll() on roleOu.RoleId equals userOuRoles.Id
                where userOu.UserId == userId
                select userOuRoles.Name);

            return userRoles.Union(userOrganizationUnitRoles).ToList();
        }

        public virtual async Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            return (await GetRolesAsync(user)).Any(r => r == roleName);
        }

        #endregion

        #region IUserPermissionStore

        public virtual async Task AddPermissionAsync(TUser user, PermissionGrantInfo permissionGrant)
        {
            if (await HasPermissionAsync(user.Id, permissionGrant))
            {
                return;
            }

            await _userPermissionSettingRepository.InsertAsync(
                new UserPermissionSetting
                {
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    Name = permissionGrant.Name,
                    IsGranted = permissionGrant.IsGranted
                });
        }

        public virtual void AddPermission(TUser user, PermissionGrantInfo permissionGrant)
        {
            if (HasPermission(user.Id, permissionGrant))
            {
                return;
            }

            _userPermissionSettingRepository.Insert(
                new UserPermissionSetting
                {
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    Name = permissionGrant.Name,
                    IsGranted = permissionGrant.IsGranted
                });
        }

        public virtual async Task RemovePermissionAsync(TUser user, PermissionGrantInfo permissionGrant)
        {
            await _userPermissionSettingRepository.DeleteAsync(
                permissionSetting => permissionSetting.UserId == user.Id &&
                                     permissionSetting.Name == permissionGrant.Name &&
                                     permissionSetting.IsGranted == permissionGrant.IsGranted
            );
        }

        public virtual void RemovePermission(TUser user, PermissionGrantInfo permissionGrant)
        {
            _userPermissionSettingRepository.Delete(
                permissionSetting => permissionSetting.UserId == user.Id &&
                                     permissionSetting.Name == permissionGrant.Name &&
                                     permissionSetting.IsGranted == permissionGrant.IsGranted
            );
        }

        public virtual async Task<IList<PermissionGrantInfo>> GetPermissionsAsync(long userId)
        {
            return (await _userPermissionSettingRepository.GetAllListAsync(p => p.UserId == userId))
                .Select(p => new PermissionGrantInfo(p.Name, p.IsGranted))
                .ToList();
        }

        public virtual IList<PermissionGrantInfo> GetPermissions(long userId)
        {
            return (_userPermissionSettingRepository.GetAllList(p => p.UserId == userId))
                .Select(p => new PermissionGrantInfo(p.Name, p.IsGranted))
                .ToList();
        }

        public virtual async Task<bool> HasPermissionAsync(long userId, PermissionGrantInfo permissionGrant)
        {
            return await _userPermissionSettingRepository.FirstOrDefaultAsync(
                       p => p.UserId == userId &&
                            p.Name == permissionGrant.Name &&
                            p.IsGranted == permissionGrant.IsGranted
                   ) != null;
        }

        public virtual bool HasPermission(long userId, PermissionGrantInfo permissionGrant)
        {
            return _userPermissionSettingRepository.FirstOrDefault(
                       p => p.UserId == userId &&
                            p.Name == permissionGrant.Name &&
                            p.IsGranted == permissionGrant.IsGranted
                   ) != null;
        }

        public virtual async Task RemoveAllPermissionSettingsAsync(TUser user)
        {
            await _userPermissionSettingRepository.DeleteAsync(s => s.UserId == user.Id);
        }

        public virtual void RemoveAllPermissionSettings(TUser user)
        {
            _userPermissionSettingRepository.Delete(s => s.UserId == user.Id);
        }

        #endregion

        #region IUserLockoutStore

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return Task.FromResult(
                user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset()
            );
        }

        public DateTimeOffset GetLockoutEndDate(TUser user)
        {
            return 
                user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset();
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd == DateTimeOffset.MinValue ? new DateTime?() : lockoutEnd.UtcDateTime;
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(++user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public void ResetAccessFailedCount(TUser user)
        {
            user.AccessFailedCount = 0;
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task.FromResult(user.IsLockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            user.IsLockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        #endregion

        #region IUserPhoneNumberStore

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.IsPhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            user.IsPhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        #endregion

        #region IUserClaimStore

        public async Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            var userClaims = await _userClaimRepository.GetAllListAsync(uc => uc.UserId == user.Id);
            return userClaims.Select(uc => new Claim(uc.ClaimType, uc.ClaimValue)).ToList();
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            return _userClaimRepository.InsertAsync(new UserClaim(user, claim));
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            return _userClaimRepository.DeleteAsync(
                uc => uc.UserId == user.Id &&
                      uc.ClaimType == claim.Type &&
                      uc.ClaimValue == claim.Value
            );
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            //No need to dispose since using IOC.
        }

        #endregion

        #region Helpers

        protected virtual string NormalizeKey(string key)
        {
            return key.ToUpperInvariant();
        }

        private async Task<TRole> GetRoleByNameAsync(string roleName)
        {
            var normalizedName = NormalizeKey(roleName);
            var role = await _roleRepository.FirstOrDefaultAsync(r => r.NormalizedName == normalizedName);
            if (role == null)
            {
                throw new AbpException("Could not find a role with name: " + normalizedName);
            }

            return role;
        }

        #endregion

        #region IUserSecurityStampStore

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        #endregion

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            user.IsTwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.FromResult(user.IsTwoFactorEnabled);
        }

        public async Task<string> GetUserNameFromDatabaseAsync(long userId)
        {
            //note: This workaround will not be needed after fixing https://github.com/aspnetboilerplate/aspnetboilerplate/issues/1828
            var outerUow = _unitOfWorkManager.Current;
            using (var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                Scope = TransactionScopeOption.RequiresNew,
                IsTransactional = false,
                IsolationLevel = IsolationLevel.ReadUncommitted
            }))
            {
                if (outerUow != null)
                {
                    _unitOfWorkManager.Current.SetTenantId(outerUow.GetTenantId());
                }

                var user = await _userRepository.GetAsync(userId);
                await uow.CompleteAsync();
                return user.UserName;
            }
        }
    }
}
