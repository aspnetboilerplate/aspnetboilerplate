using Abp.Authorization.Users;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class UserRoleMap : EntityMap<UserRole, long>
{
    public UserRoleMap()
        : base("AbpUserRoles")
    {
        Map(x => x.TenantId);
        Map(x => x.UserId)
            .Not.Nullable();
        Map(x => x.RoleId)
            .Not.Nullable();

        this.MapCreationAudited();
    }
}