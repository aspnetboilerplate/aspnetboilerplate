using Abp.Roles;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public class RoleMap : EntityMap<Role>
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