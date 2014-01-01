using Abp.Modules.Core.Domain.Entities;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public class RoleMap : EntityMap<Role>
    {
        public RoleMap()
            : base("AbpRoles")
        {
            Map(x => x.Name);
            Map(x => x.DisplayName);
            Map(x => x.IsStatic);
        }
    }
}