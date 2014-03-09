using Abp.Domain.Entities.Mapping;
using Abp.Modules.Core.Entities.NHibernate.Mappings.Extensions;
using Abp.Security.Roles;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public abstract class RoleMapBase<TRole> : EntityMap<TRole> where TRole : AbpRole
    {
        protected RoleMapBase()
            : base("AbpRoles")
        {
            Map(x => x.Name);
            Map(x => x.DisplayName);
            this.MapAudited();
            HasMany(x => x.Permissions).KeyColumn("RoleId");
        }
    }
}