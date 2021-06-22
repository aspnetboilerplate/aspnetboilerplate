using Abp.Authorization.Users;

namespace Abp.ZeroCore.SampleApp.Core
{
    public class User : AbpUser<User>
    {
        public override string ToString()
        {
            return $"[User {Id}] {UserName}";
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress
            };

            user.SetNormalizedNames();

            return user;
        }
    }
}
