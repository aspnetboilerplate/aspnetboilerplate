using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings
{
    /// <summary>
    /// Base class for role mapping.
    /// </summary>
    public abstract class RoleMap<TTenant, TRole, TUser> : EntityMap<TRole>
        where TRole : AbpRole<TTenant, TUser>
        where TUser : AbpUser<TTenant, TUser>
        where TTenant : AbpTenant<TTenant, TUser>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected RoleMap()
            : base("AbpRoles")
        {
            Map(x => x.TenantId);
            Map(x => x.Name);
            Map(x => x.DisplayName);
            Map(x => x.IsStatic);
            Map(x => x.IsDefault);
            
            this.MapDeletionAudited();
            this.MapAudited();

            Polymorphism.Explicit();
        }
    }
}