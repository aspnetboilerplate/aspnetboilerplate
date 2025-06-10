using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Zero.Ldap.Configuration;

namespace Abp.Zero.Ldap.Authentication
{
    /// <summary>
    /// Implements <see cref="IExternalAuthenticationSource{TTenant,TUser}"/> to authenticate users from LDAP.
    /// Extend this class using application's User and Tenant classes as type parameters.
    /// Also, all needed methods can be overridden and changed upon your needs.
    /// </summary>
    /// <typeparam name="TTenant">Tenant type</typeparam>
    /// <typeparam name="TUser">User type</typeparam>
    public abstract class LdapAuthenticationSource<TTenant, TUser> :
        DefaultExternalAuthenticationSource<TTenant, TUser>,
        ITransientDependency
        where TTenant : AbpTenant<TUser>
        where TUser : AbpUserBase, new()
    {
        /// <summary>
        /// LDAP
        /// </summary>
        public const string SourceName = "LDAP";

        public override string Name => SourceName;

        private readonly ILdapSettings _settings;
        private readonly IAbpZeroLdapModuleConfig _ldapModuleConfig;

        protected LdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
        {
            _settings = settings;
            _ldapModuleConfig = ldapModuleConfig;
        }

        /// <inheritdoc/>
        public override async Task<bool> TryAuthenticateAsync(string userNameOrEmailAddress, string plainPassword,
            TTenant tenant)
        {
            if (!_ldapModuleConfig.IsEnabled || !(await _settings.GetIsEnabled(tenant?.Id)))
            {
                return false;
            }

            using (var principalContext = await CreatePrincipalContext(tenant, userNameOrEmailAddress))
            {
                return ValidateCredentials(principalContext, userNameOrEmailAddress, plainPassword);
            }
        }

        /// <inheritdoc/>
        public override async Task<TUser> CreateUserAsync(string userNameOrEmailAddress, TTenant tenant)
        {
            await CheckIsEnabled(tenant);

            var user = await base.CreateUserAsync(userNameOrEmailAddress, tenant);

            using (var principalContext = await CreatePrincipalContext(tenant, user))
            {
                var userPrincipal = FindUserPrincipalByIdentity(principalContext, userNameOrEmailAddress);

                if (userPrincipal == null)
                {
                    throw new AbpException("Unknown LDAP user: " + userNameOrEmailAddress);
                }

                UpdateUserFromPrincipal(user, userPrincipal);

                user.IsEmailConfirmed = true;
                user.IsActive = true;

                return user;
            }
        }

        public override async Task UpdateUserAsync(TUser user, TTenant tenant)
        {
            await CheckIsEnabled(tenant);

            await base.UpdateUserAsync(user, tenant);

            using (var principalContext = await CreatePrincipalContext(tenant, user))
            {
                var userPrincipal = FindUserPrincipalByIdentity(principalContext, user.UserName);

                if (userPrincipal == null)
                {
                    throw new AbpException("Unknown LDAP user: " + user.UserName);
                }

                UpdateUserFromPrincipal(user, userPrincipal);
            }
        }

        protected virtual bool ValidateCredentials(PrincipalContext principalContext, string userNameOrEmailAddress,
            string plainPassword)
        {
            return principalContext.ValidateCredentials(userNameOrEmailAddress, plainPassword,
                ContextOptions.Negotiate);
        }

        protected virtual void UpdateUserFromPrincipal(TUser user, UserPrincipal userPrincipal)
        {
            user.UserName = GetUsernameFromUserPrincipal(userPrincipal);

            user.Name = userPrincipal.GivenName;
            user.Surname = userPrincipal.Surname;
            user.EmailAddress = userPrincipal.EmailAddress;

            if (userPrincipal.Enabled.HasValue)
            {
                user.IsActive = userPrincipal.Enabled.Value;
            }
        }

        protected virtual UserPrincipal FindUserPrincipalByIdentity(
            PrincipalContext principalContext,
            string userNameOrEmailAddress)
        {
            var userPrincipal =
                UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, userNameOrEmailAddress) ??
                UserPrincipal.FindByIdentity(principalContext, IdentityType.UserPrincipalName, userNameOrEmailAddress);

            return userPrincipal;
        }

        protected virtual string GetUsernameFromUserPrincipal(UserPrincipal userPrincipal)
        {
            return userPrincipal.SamAccountName.IsNullOrEmpty()
                ? userPrincipal.UserPrincipalName
                : userPrincipal.SamAccountName;
        }

        protected virtual Task<PrincipalContext> CreatePrincipalContext(TTenant tenant, string userNameOrEmailAddress)
        {
            return CreatePrincipalContext(tenant);
        }

        protected virtual Task<PrincipalContext> CreatePrincipalContext(TTenant tenant, TUser user)
        {
            return CreatePrincipalContext(tenant);
        }

        protected virtual async Task<PrincipalContext> CreatePrincipalContext(TTenant tenant)
        {
            var useSsl = await _settings.GetUseSsl(tenant?.Id);
            var contextType = await _settings.GetContextType(tenant?.Id);
            
            var options = useSsl
                ? ContextOptions.SecureSocketLayer | ContextOptions.Negotiate
                : GetDefaultOptionForStore(contextType);

            return new PrincipalContext(
                contextType,
                ConvertToNullIfEmpty(await _settings.GetDomain(tenant?.Id)),
                ConvertToNullIfEmpty(await _settings.GetContainer(tenant?.Id)),
                options,
                ConvertToNullIfEmpty(await _settings.GetUserName(tenant?.Id)),
                ConvertToNullIfEmpty(await _settings.GetPassword(tenant?.Id))
            );
        }

        private ContextOptions GetDefaultOptionForStore(ContextType contextType)
        {
            if (contextType == ContextType.Machine)
            {
                return ContextOptions.Negotiate;
            }

            return ContextOptions.Negotiate | ContextOptions.Signing | ContextOptions.Sealing;
        }

        protected virtual async Task CheckIsEnabled(TTenant tenant)
        {
            if (!_ldapModuleConfig.IsEnabled)
            {
                throw new AbpException("Ldap Authentication module is disabled globally!");
            }

            var tenantId = tenant?.Id;
            if (!await _settings.GetIsEnabled(tenantId))
            {
                throw new AbpException("Ldap Authentication is disabled for given tenant (id:" + tenantId +
                                       ")! You can enable it by setting '" + LdapSettingNames.IsEnabled + "' to true");
            }
        }

        protected static string ConvertToNullIfEmpty(string str)
        {
            return str.IsNullOrWhiteSpace()
                ? null
                : str;
        }
    }
}