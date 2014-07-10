using Abp.Security.Users;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    public class AbpUserRepository : CoreModuleEfRepositoryBase<AbpUser, long>, IAbpUserRepository
    {
        public void UpdatePassword(long userId, string password)
        {
            var user = Load(userId);
            user.Password = password;
        }

        public void UpdateEmail(long userId, string emailAddress)
        {
            var user = Load(userId);
            user.EmailAddress = emailAddress;
        }

        public void UpdateIsEmailConfirmed(long userId, bool confirmed)
        {
            var user = Load(userId);
            user.IsEmailConfirmed = confirmed;
        }
    }
}