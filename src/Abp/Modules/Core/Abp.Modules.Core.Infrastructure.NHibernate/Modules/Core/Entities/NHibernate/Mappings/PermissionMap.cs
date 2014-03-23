using Abp.Domain.Entities.Mapping;
using Abp.Security.Roles;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public class PermissionMap : EntityMap<Permission>
    {
        public PermissionMap()
            : base("AbpPermissions")
        {
            Map(x => x.RoleId);
            Map(x => x.UserId);
            Map(x => x.PermissionName);
            Map(x => x.IsGranted);
            this.MapCreationAudited();
        }
    }
}