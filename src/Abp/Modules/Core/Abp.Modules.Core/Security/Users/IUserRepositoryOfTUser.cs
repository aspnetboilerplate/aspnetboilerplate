using Abp.Domain.Repositories;

namespace Abp.Security.Users
{
    /// <summary>
    /// Used to perform <see cref="User"/> related database operations.
    /// </summary>
    public interface IUserRepository<TUser> : IRepository<TUser> where TUser : User
    {
        void UpdatePassword(int userId, string password);
        void UpdateEmail(int userId, string emailAddress);
    }
}
