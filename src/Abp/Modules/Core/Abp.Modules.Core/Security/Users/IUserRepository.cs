using Abp.Domain.Repositories;

namespace Abp.Security.Users
{
    /// <summary>
    /// Used to perform <see cref="AbpUser"/> related database operations.
    /// </summary>
    public interface IUserRepository<TUser> : IRepository<TUser> where TUser : AbpUser
    {
        void UpdatePassword(int userId, string password);

        void UpdateEmail(int userId, string emailAddress);

        void UpdateIsEmailConfirmed(int userId, bool confirmed);
    }
}
