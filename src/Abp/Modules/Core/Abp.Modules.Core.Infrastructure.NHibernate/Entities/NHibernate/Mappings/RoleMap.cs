using Abp.Security.Roles;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public class RoleMap : EntityMap<AbpRole>
    {
        public RoleMap()
            : base("AbpRoles")
        {
            Map(x => x.Name);
            Map(x => x.DisplayName);
            HasMany(x => x.Permissions).KeyColumn("RoleId");
        }
    }
}