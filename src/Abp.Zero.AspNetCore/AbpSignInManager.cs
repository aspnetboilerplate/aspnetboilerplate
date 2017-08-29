using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.Zero.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;

namespace Abp.Zero.AspNetCore
{
    public abstract class AbpSignInManager<TTenant, TRole, TUser> : ITransientDependency
           where TTenant : AbpTenant<TUser>
           where TRole : AbpRole<TUser>, new()
           where TUser : AbpUser<TUser>
    {
        public AbpUserManager<TRole, TUser> UserManager { get; set; }

        public AuthenticationManager AuthenticationManager => _contextAccessor.HttpContext.Authentication;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ISettingManager _settingManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAbpZeroAspNetCoreConfiguration _configuration;

        protected AbpSignInManager(
            AbpUserManager<TRole, TUser> userManager,
            IHttpContextAccessor contextAccessor,
            ISettingManager settingManager,
            IUnitOfWorkManager unitOfWorkManager,
            IAbpZeroAspNetCoreConfiguration configuration)
        {
            UserManager = userManager;
            _contextAccessor = contextAccessor;
            _settingManager = settingManager;
            _unitOfWorkManager = unitOfWorkManager;
            _configuration = configuration;
        }

        /// <summary>
        /// This method can return two results:
        /// <see cref="SignInStatus.Success"/> indicates that user has successfully signed in.
        /// <see cref="SignInStatus.RequiresVerification"/> indicates that user has successfully signed in.
        /// </summary>
        /// <param name="loginResult">The login result received from <see cref="AbpLogInManager{TTenant,TRole,TUser}"/> Should be Success.</param>
        /// <param name="isPersistent">True to use persistent cookie.</param>
        /// <param name="rememberBrowser">Remember user's browser (and not use two factor auth again) or not.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">loginResult.Result should be success in order to sign in!</exception>
        [UnitOfWork]
        public virtual async Task<SignInStatus> SignInOrTwoFactorAsync(AbpLoginResult<TTenant, TUser> loginResult, bool isPersistent, bool? rememberBrowser = null)
        {
            if (loginResult.Result != AbpLoginResultType.Success)
            {
                throw new ArgumentException("loginResult.Result should be success in order to sign in!");
            }

            using (_unitOfWorkManager.Current.SetTenantId(loginResult.Tenant?.Id))
            {
                if (IsTrue(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled, loginResult.Tenant?.Id))
                {
                    UserManager.As<AbpUserManager<TRole, TUser>>().RegisterTwoFactorProviders(loginResult.Tenant?.Id);

                    if (await UserManager.GetTwoFactorEnabledAsync(loginResult.User.Id))
                    {
                        if ((await UserManager.GetValidTwoFactorProvidersAsync(loginResult.User.Id)).Count > 0)
                        {
                            if (!await TwoFactorBrowserRememberedAsync(loginResult.User.Id.ToString(), loginResult.User.TenantId) ||
                                rememberBrowser == false)
                            {
                                var claimsIdentity = new ClaimsIdentity(_configuration.TwoFactorAuthenticationScheme);

                                claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginResult.User.Id.ToString()));

                                if (loginResult.Tenant != null)
                                {
                                    claimsIdentity.AddClaim(new Claim(AbpClaimTypes.TenantId, loginResult.Tenant.Id.ToString()));
                                }

                                await AuthenticationManager.SignInAsync(
                                    _configuration.TwoFactorAuthenticationScheme,
                                    CreateIdentityForTwoFactor(
                                        loginResult.User,
                                        _configuration.TwoFactorAuthenticationScheme
                                    )
                                );

                                return SignInStatus.RequiresVerification;
                            }
                        }
                    }
                }

                await SignInAsync(loginResult, isPersistent, rememberBrowser);
                return SignInStatus.Success;
            }
        }

        /// <param name="loginResult">The login result received from <see cref="AbpLogInManager{TTenant,TRole,TUser}"/> Should be Success.</param>
        /// <param name="isPersistent">True to use persistent cookie.</param>
        /// <param name="rememberBrowser">Remember user's browser (and not use two factor auth again) or not.</param>
        [UnitOfWork]
        public virtual async Task SignInAsync(AbpLoginResult<TTenant, TUser> loginResult, bool isPersistent, bool? rememberBrowser = null)
        {
            if (loginResult.Result != AbpLoginResultType.Success)
            {
                throw new ArgumentException("loginResult.Result should be success in order to sign in!");
            }

            using (_unitOfWorkManager.Current.SetTenantId(loginResult.Tenant?.Id))
            {
                await SignOutAllAsync();

                if (rememberBrowser == null)
                {
                    rememberBrowser = IsTrue(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled, loginResult.Tenant?.Id);
                }

                if (rememberBrowser == true)
                {
                    await SignOutAllAndSignInAsync(loginResult.Identity, isPersistent);
                    await AuthenticationManager.SignInAsync(
                        _configuration.TwoFactorRememberBrowserAuthenticationScheme,
                        CreateIdentityForTwoFactor(
                            loginResult.User,
                            _configuration.TwoFactorRememberBrowserAuthenticationScheme
                        )
                    );
                }
                else
                {
                    await SignOutAllAndSignInAsync(loginResult.Identity, isPersistent);
                }
            }
        }

        private static ClaimsPrincipal CreateIdentityForTwoFactor(TUser user, string authType)
        {
            var claimsIdentity = new ClaimsIdentity(authType);

            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

            if (user.TenantId.HasValue)
            {
                claimsIdentity.AddClaim(new Claim(AbpClaimTypes.TenantId, user.TenantId.Value.ToString()));
            }

            return new ClaimsPrincipal(claimsIdentity);
        }

        public virtual async Task SignInAsync(TUser user, bool isPersistent, bool? rememberBrowser = null)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var userIdentity = await UserManager.CreateIdentityAsync(user, _configuration.AuthenticationScheme);

                await SignOutAllAsync();

                if (rememberBrowser == null)
                {
                    rememberBrowser = IsTrue(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled, user.TenantId);
                }

                if (rememberBrowser == true)
                {
                    await SignOutAllAndSignInAsync(userIdentity, isPersistent);
                    await AuthenticationManager.SignInAsync(
                        _configuration.TwoFactorRememberBrowserAuthenticationScheme,
                        CreateIdentityForTwoFactor(
                            user,
                            _configuration.TwoFactorRememberBrowserAuthenticationScheme
                        )
                    );
                }
                else
                {
                    await SignOutAllAndSignInAsync(userIdentity, isPersistent);
                }
            }
        }

        public virtual async Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            var userId = await GetVerifiedUserIdAsync();
            if (userId <= 0)
            {
                return SignInStatus.Failure;
            }

            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return SignInStatus.Failure;
            }

            if (await UserManager.IsLockedOutAsync(user.Id))
            {
                return SignInStatus.LockedOut;
            }

            if (await UserManager.VerifyTwoFactorTokenAsync(user.Id, provider, code))
            {
                await UserManager.ResetAccessFailedCountAsync(user.Id);
                await SignInAsync(user, isPersistent, rememberBrowser);
                return SignInStatus.Success;
            }

            await UserManager.AccessFailedAsync(user.Id);
            return SignInStatus.Failure;
        }

        public virtual async Task<bool> SendTwoFactorCodeAsync(string provider)
        {
            var userId = await GetVerifiedUserIdAsync();
            if (userId <= 0)
            {
                return false;
            }

            var token = await UserManager.GenerateTwoFactorTokenAsync(userId, provider);
            var identityResult = await UserManager.NotifyTwoFactorTokenAsync(userId, provider, token);
            return identityResult == IdentityResult.Success;
        }

        public async Task<long> GetVerifiedUserIdAsync()
        {
            var authenticateResult = await AuthenticationManager.AuthenticateAsync(_configuration.TwoFactorAuthenticationScheme);
            return Convert.ToInt64(IdentityExtensions.GetUserId(authenticateResult.Identity) ?? "0");
        }

        public virtual async Task<int?> GetVerifiedTenantIdAsync()
        {
            var authenticateResult = await AuthenticationManager.AuthenticateAsync(_configuration.TwoFactorAuthenticationScheme);
            return AbpZeroClaimsIdentityHelper.GetTenantId(authenticateResult?.Identity);
        }

        public async Task<bool> TwoFactorBrowserRememberedAsync(string userId, int? tenantId)
        {
            var result = await AuthenticationManager.AuthenticateAsync(_configuration.TwoFactorRememberBrowserAuthenticationScheme);
            if (result?.Identity == null)
            {
                return false;
            }

            if (IdentityExtensions.GetUserId(result.Identity) != userId)
            {
                return false;
            }

            if (AbpZeroClaimsIdentityHelper.GetTenantId(result.Identity) != tenantId)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> HasBeenVerifiedAsync()
        {
            return await GetVerifiedUserIdAsync() > 0;
        }

        private bool IsTrue(string settingName, int? tenantId)
        {
            return tenantId == null
                ? _settingManager.GetSettingValueForApplication<bool>(settingName)
                : _settingManager.GetSettingValueForTenant<bool>(settingName, tenantId.Value);
        }

        public async Task SignOutAllAsync()
        {
            await AuthenticationManager.SignOutAsync(_configuration.AuthenticationScheme);
            await AuthenticationManager.SignOutAsync(_configuration.TwoFactorAuthenticationScheme);
        }

        public async Task SignOutAllAndSignInAsync(ClaimsIdentity identity, bool rememberMe = false)
        {
            await SignOutAllAsync();
            await AuthenticationManager.SignInAsync(
                _configuration.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = rememberMe
                }
            );
        }

        public async Task<ExternalLoginUserInfo> GetExternalLoginUserInfo(string authSchema)
        {
            var authInfo = await AuthenticationManager.GetAuthenticateInfoAsync(authSchema);
            if (authInfo == null)
            {
                return null;
            }

            var claims = authInfo.Principal.Claims.ToList();

            var userInfo = new ExternalLoginUserInfo
            {
                LoginInfo = new UserLoginInfo(
                    authInfo.Properties.Items["LoginProvider"],
                    authInfo.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                )
            };

            var givennameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
            if (givennameClaim != null && !givennameClaim.Value.IsNullOrEmpty())
            {
                userInfo.Name = givennameClaim.Value;
            }

            var surnameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
            if (surnameClaim != null && !surnameClaim.Value.IsNullOrEmpty())
            {
                userInfo.Surname = surnameClaim.Value;
            }

            if (userInfo.Name == null || userInfo.Surname == null)
            {
                var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (nameClaim != null)
                {
                    var nameSurName = nameClaim.Value;
                    if (!nameSurName.IsNullOrEmpty())
                    {
                        var lastSpaceIndex = nameSurName.LastIndexOf(' ');
                        if (lastSpaceIndex < 1 || lastSpaceIndex > (nameSurName.Length - 2))
                        {
                            userInfo.Name = userInfo.Surname = nameSurName;
                        }
                        else
                        {
                            userInfo.Name = nameSurName.Substring(0, lastSpaceIndex);
                            userInfo.Surname = nameSurName.Substring(lastSpaceIndex);
                        }
                    }
                }
            }

            var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim != null)
            {
                userInfo.EmailAddress = emailClaim.Value;
            }

            return userInfo;
        }
    }
}
