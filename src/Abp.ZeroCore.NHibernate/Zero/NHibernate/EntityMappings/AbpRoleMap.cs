using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

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
        Map(x => x.Name)
            .Not.Nullable()
            .Length(AbpRoleBase.MaxNameLength);
        Map(x => x.DisplayName).Not.Nullable().Length(AbpRoleBase.MaxDisplayNameLength);
        Map(x => x.IsStatic).Not.Nullable();
        Map(x => x.IsDefault).Not.Nullable();

        Map(x => x.NormalizedName).Not.Nullable().Length(AbpRoleBase.MaxNameLength);
        Map(x => x.ConcurrencyStamp)
            .Length(AbpRole<TUser>.MaxConcurrencyStampLength);


        HasMany(x => x.Claims)
            .Inverse()
            .Cascade.AllDeleteOrphan()
            .KeyColumn("RoleId");
        HasMany(x => x.Permissions)
            .Inverse()
            .Cascade.AllDeleteOrphan()
            .KeyColumn("RoleId");

        this.MapFullAudited();

        Polymorphism.Explicit();
    }
}