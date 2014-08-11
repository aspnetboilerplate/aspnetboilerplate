using Abp.Domain.Entities.Mapping;
using Abp.Security.Permissions;

namespace Abp.Zero.Entities.NHibernate.Mappings
{
    public class PermissionMap : EntityMap<Permission, long>
    {
        public PermissionMap()
            : base("AbpPermissions")
        {
            Map(x => x.RoleId);
            Map(x => x.UserId);
            Map(x => x.Name);
            Map(x => x.IsGranted);
            this.MapCreationAudited();
        }
    }
}