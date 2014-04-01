using Abp.Domain.Repositories.EntityFramework;
using Abp.Security.Users;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    /// <summary>
    /// Implements <see cref="IUserRepository{TUser}"/> for NHibernate.
    /// </summary>
    public abstract class UserRepositoryBase<TUser> : EfRepositoryBase<TUser>, IUserRepository<TUser> where TUser : AbpUser,new()
    {
        public void UpdatePassword(int userId, string password)
        {
            var user = Load(userId);
            user.Password = password; //TODO: Test
        }

        public void UpdateEmail(int userId, string emailAddress)
        {
            var user = Load(userId);
            user.EmailAddress = emailAddress; //TODO: Test
        }

        public void UpdateIsEmailConfirmed(int userId, bool confirmed)
        {
            var user = Load(userId);
            user.IsEmailConfirmed = confirmed; //TODO: Test
        }
    }
}
