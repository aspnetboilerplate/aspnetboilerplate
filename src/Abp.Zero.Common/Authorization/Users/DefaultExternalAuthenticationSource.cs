using System.Threading.Tasks;
using Abp.MultiTenancy;

namespace Abp.Authorization.Users
{
    /// <summary>
    /// This is a helper base class to easily update <see cref="IExternalAuthenticationSource{TTenant,TUser}"/>.
    /// Implements some methods as default but you can override all methods.
    /// </summary>
    /// <typeparam name="TTenant">Tenant type</typeparam>
    /// <typeparam name="TUser">User type</typeparam>
    public abstract class DefaultExternalAuthenticationSource<TTenant, TUser> : IExternalAuthenticationSource<TTenant, TUser>
        where TTenant : AbpTenant<TUser>
        where TUser : AbpUserBase, new()
    {
        /// <inheritdoc/>
        public abstract string Name { get; }

        /// <inheritdoc/>
        public abstract Task<bool> TryAuthenticateAsync(string userNameOrEmailAddress, string plainPassword, TTenant tenant);

        /// <inheritdoc/>
        public virtual Task<TUser> CreateUserAsync(string userNameOrEmailAddress, TTenant tenant)
        {
            return Task.FromResult(
                new TUser
                {
                    UserName = userNameOrEmailAddress,
                    Name = userNameOrEmailAddress,
                    Surname = userNameOrEmailAddress,
                    EmailAddress = userNameOrEmailAddress,
                    IsEmailConfirmed = true,
                    IsActive = true
                });
        }

        /// <inheritdoc/>
        public virtual Task UpdateUserAsync(TUser user, TTenant tenant)
        {
            return Task.FromResult(0);
        }
    }
}