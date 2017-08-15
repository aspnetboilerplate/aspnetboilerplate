using Abp.Authorization.Users;

namespace Abp.Zero.SampleApp.Users
{
    public class User : AbpUser<User>
    {
        public override string ToString()
        {
            return string.Format("[User {0}] {1}", Id, UserName);
        }
    }
}