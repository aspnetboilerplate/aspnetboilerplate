using Abp.Authorization.Users;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings
{
    public class UserRoleMap : EntityMap<UserRole, long>
    {
        public UserRoleMap()
            : base("AbpUserRoles")
        {
            Map(x => x.UserId);
            Map(x => x.RoleId);
            
            this.MapCreationAudited();
        }
    }
}
