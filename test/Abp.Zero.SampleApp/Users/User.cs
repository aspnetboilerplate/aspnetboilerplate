using Abp.Authorization.Users;

namespace Abp.Zero.SampleApp.Users
{
    public class User : AbpUser<User>
    {
        public override string ToString()
        {
            return $"[User {Id}] {UserName}";
        }
    }
}
