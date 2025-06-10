using Abp.Authorization.Users;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class UserLoginMap : EntityMap<UserLogin, long>
{
    public UserLoginMap()
        : base("AbpUserLogins")
    {
        Map(x => x.UserId)
            .Not.Nullable();
        Map(x => x.LoginProvider)
            .Length(UserLogin.MaxLoginProviderLength)
            .Not.Nullable();
        Map(x => x.ProviderKey)
            .Length(UserLogin.MaxProviderKeyLength)
            .Not.Nullable();

        Map(x => x.TenantId);
    }
}