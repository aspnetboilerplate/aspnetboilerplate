using Abp.Domain.Repositories;

namespace Abp.Security.Users
{
    public interface IAbpUserRepository : IRepository<AbpUser>
    {
        void UpdatePassword(int userId, string password);

        void UpdateEmail(int userId, string emailAddress);

        void UpdateIsEmailConfirmed(int userId, bool confirmed);
    }
}