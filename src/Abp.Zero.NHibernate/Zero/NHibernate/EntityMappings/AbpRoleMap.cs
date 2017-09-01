using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings
{
    /// <summary>
    /// Base class for role mapping.
    /// </summary>
    public abstract class AbpRoleMap<TRole, TUser> : EntityMap<TRole>
        where TRole : AbpRole<TUser>
        where TUser : AbpUser<TUser>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpRoleMap()
            : base("AbpRoles")
        {
            Map(x => x.TenantId);
            Map(x => x.Name);
            Map(x => x.DisplayName);
            Map(x => x.IsStatic);
            Map(x => x.IsDefault);
            
            this.MapFullAudited();

            Polymorphism.Explicit();
        }
    }
}