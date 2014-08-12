using Abp.Application.Authorization.Roles;
using Abp.Domain.Entities.Mapping;

namespace Abp.Zero.EntityMappings
{
    public class UserRoleMap : EntityMap<UserRole, long>
    {
        public UserRoleMap()
            : base("AbpUserRoles")
        {
            Map(x => x.UserId);
            Map(x => x.RoleId);
            
            References(x => x.User).Column("UserId").LazyLoad();
            References(x => x.Role).Column("RoleId").LazyLoad();

            this.MapCreationAudited();
        }
    }
}
