using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class UserLoginAttemptMap : EntityMap<UserLoginAttempt, long>
{
    public UserLoginAttemptMap()
        : base("AbpUserLoginAttempts")
    {
        Map(x => x.TenantId);
        Map(x => x.TenancyName)
            .Length(UserLoginAttempt.MaxTenancyNameLength);
        Map(x => x.UserId);
        Map(x => x.UserNameOrEmailAddress)
            .Length(UserLoginAttempt.MaxUserNameOrEmailAddressLength);
        Map(x => x.ClientIpAddress)
            .Length(UserLoginAttempt.MaxClientIpAddressLength);
        Map(x => x.ClientName)
            .Length(UserLoginAttempt.MaxClientNameLength);
        Map(x => x.BrowserInfo)
            .Length(UserLoginAttempt.MaxBrowserInfoLength);
        Map(x => x.Result)
            .CustomType<AbpLoginResultType>()
            .Not.Nullable();

        this.MapCreationTime();
    }
}