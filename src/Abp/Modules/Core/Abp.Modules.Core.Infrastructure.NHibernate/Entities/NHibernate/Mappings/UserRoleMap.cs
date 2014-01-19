using Abp.Roles;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public class UserRoleMap : EntityMap<UserRole>
    {
        public UserRoleMap()
            : base("AbpUserRoles")
        {
            References(x => x.User).Column("UserId").LazyLoad();
            References(x => x.Role).Column("RoleId").LazyLoad();
        }
    }
}
