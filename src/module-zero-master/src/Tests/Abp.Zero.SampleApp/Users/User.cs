using Abp.Authorization.Users;
using Abp.Zero.SampleApp.MultiTenancy;

namespace Abp.Zero.SampleApp.Users
{
    public class User : AbpUser<Tenant, User>
    {
        public override string ToString()
        {
            return string.Format("[User {0}] {1}", Id, UserName);
        }
    }
}