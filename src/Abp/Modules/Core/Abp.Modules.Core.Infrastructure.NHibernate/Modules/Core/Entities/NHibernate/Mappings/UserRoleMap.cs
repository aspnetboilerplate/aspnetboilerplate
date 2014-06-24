using Abp.Domain.Entities.Mapping;
using Abp.Security.Roles;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
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
