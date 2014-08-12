using Abp.Application.Authorization.Roles;
using Abp.Domain.Entities.Mapping;

namespace Abp.Zero.EntityMappings
{
    public abstract class RoleMapBase<TRole> : EntityMap<TRole> where TRole : AbpRole
    {
        protected RoleMapBase()
            : base("AbpRoles")
        {
            Map(x => x.Name);
            Map(x => x.DisplayName);

            HasMany(x => x.Permissions).KeyColumn("RoleId");
            
            this.MapAudited();

            Polymorphism.Explicit();
        }
    }
}