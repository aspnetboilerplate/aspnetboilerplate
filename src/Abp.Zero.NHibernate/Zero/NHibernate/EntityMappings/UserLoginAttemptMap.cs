using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings
{
    public class UserLoginAttemptMap : EntityMap<UserLoginAttempt, long>
    {
        public UserLoginAttemptMap() 
            : base("AbpUserLoginAttempts")
        {
            Map(x => x.TenantId);
            Map(x => x.TenancyName);
            Map(x => x.UserId);
            Map(x => x.UserNameOrEmailAddress);
            Map(x => x.ClientIpAddress);
            Map(x => x.ClientName);
            Map(x => x.BrowserInfo);
            Map(x => x.Result).CustomType<AbpLoginResultType>();

            this.MapCreationTime();
        }
    }
}