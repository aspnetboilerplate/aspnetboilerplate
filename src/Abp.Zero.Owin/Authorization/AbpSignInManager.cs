using System;
using System.Security.Claims;
using System.Threading.Tasks;
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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Abp.Authorization
{
    public abstract class AbpSignInManager<TTenant, TRole, TUser> : SignInManager<TUser, long>, ITransientDependency
        where TTenant : AbpTenant<TUser>
        where TRole : AbpRole<TUser>, new()
        where TUser : AbpUser<TUser>
    {
        private readonly ISettingManager _settingManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        protected AbpSignInManager(
            AbpUserManager<TRole, TUser> userManager,
            IAuthenticationManager authenticationManager,
            ISettingManager settingManager,
            IUnitOfWorkManager unitOfWorkManager)
            : base(
                userManager,
                authenticationManager)
        {
            _settingManager = settingManager;
            _unitOfWorkManager = unitOfWorkManager;
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
        public virtual async Task<SignInStatus> SignInOrTwoFactor(AbpLoginResult<TTenant, TUser> loginResult,
            bool isPersistent, bool? rememberBrowser = null)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if (loginResult.Result != AbpLoginResultType.Success)
                {
                    throw new ArgumentException("loginResult.Result should be success in order to sign in!");
                }

                using (_unitOfWorkManager.Current.SetTenantId(loginResult.Tenant?.Id))
                {
                    if (IsTrue(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled, loginResult.Tenant?.Id))
                    {
                        UserManager.As<AbpUserManager<TRole, TUser>>()
                            .RegisterTwoFactorProviders(loginResult.Tenant?.Id);

                        if (await UserManager.GetTwoFactorEnabledAsync(loginResult.User.Id))
                        {
                            if ((await UserManager.GetValidTwoFactorProvidersAsync(loginResult.User.Id)).Count > 0)
                            {
                                if (!await AuthenticationManager.TwoFactorBrowserRememberedAsync(loginResult.User.Id
                                        .ToString()) ||
                                    rememberBrowser == false)
                                {
                                    var claimsIdentity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);

                                    claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier,
                                        loginResult.User.Id.ToString()));

                                    if (loginResult.Tenant != null)
                                    {
                                        claimsIdentity.AddClaim(new Claim(AbpClaimTypes.TenantId,
                                            loginResult.Tenant.Id.ToString()));
                                    }

                                    AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = true},
                                        claimsIdentity);
                                    return SignInStatus.RequiresVerification;
                                }
                            }
                        }
                    }

                    SignIn(loginResult, isPersistent, rememberBrowser);
                    return SignInStatus.Success;
                }
            });
        }

        /// <param name="loginResult">The login result received from <see cref="AbpLogInManager{TTenant,TRole,TUser}"/> Should be Success.</param>
        /// <param name="isPersistent">True to use persistent cookie.</param>
        /// <param name="rememberBrowser">Remember user's browser (and not use two factor auth again) or not.</param>
        public virtual void SignIn(AbpLoginResult<TTenant, TUser> loginResult, bool isPersistent,
            bool? rememberBrowser = null)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                if (loginResult.Result != AbpLoginResultType.Success)
                {
                    throw new ArgumentException("loginResult.Result should be success in order to sign in!");
                }

                using (_unitOfWorkManager.Current.SetTenantId(loginResult.Tenant?.Id))
                {
                    AuthenticationManager.SignOut(
                        DefaultAuthenticationTypes.ExternalCookie,
                        DefaultAuthenticationTypes.TwoFactorCookie
                    );

                    if (rememberBrowser == null)
                    {
                        rememberBrowser = IsTrue(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled,
                            loginResult.Tenant?.Id);
                    }

                    if (rememberBrowser == true)
                    {
                        var rememberBrowserIdentity =
                            AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(loginResult.User.Id.ToString());
                        AuthenticationManager.SignIn(
                            new AuthenticationProperties
                            {
                                IsPersistent = isPersistent
                            },
                            loginResult.Identity,
                            rememberBrowserIdentity
                        );
                    }
                    else
                    {
                        AuthenticationManager.SignIn(
                            new AuthenticationProperties
                            {
                                IsPersistent = isPersistent
                            },
                            loginResult.Identity
                        );
                    }
                }
            });
        }

        public virtual async Task<int?> GetVerifiedTenantIdAsync()
        {
            var authenticateResult = await AuthenticationManager.AuthenticateAsync(
                DefaultAuthenticationTypes.TwoFactorCookie
            );

            return authenticateResult?.Identity?.GetTenantId();
        }

        private bool IsTrue(string settingName, int? tenantId)
        {
            return tenantId == null
                ? _settingManager.GetSettingValueForApplication<bool>(settingName)
                : _settingManager.GetSettingValueForTenant<bool>(settingName, tenantId.Value);
        }
    }
}
