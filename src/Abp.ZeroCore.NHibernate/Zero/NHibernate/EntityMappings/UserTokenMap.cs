using Abp.Authorization.Users;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class UserTokenMap : EntityMap<UserToken, long>
{
    public UserTokenMap() : base("AbpUserTokens")
    {
        Map(x => x.TenantId);
        Map(x => x.UserId)
            .Not.Nullable();
        Map(x => x.LoginProvider)
            .Length(UserToken.MaxLoginProviderLength);
        Map(x => x.Name)
            .Length(UserToken.MaxNameLength);
        Map(x => x.Value)
            .Length(UserToken.MaxValueLength);
        Map(x => x.ExpireDate);
    }
}