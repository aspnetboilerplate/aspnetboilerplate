using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

/// <summary>
/// Base class to map classes derived from <see cref="AbpTenant{TUser}"/>
/// </summary>
/// <typeparam name="TTenant">Tenant type</typeparam>
/// <typeparam name="TUser">User type</typeparam>
public abstract class AbpTenantMap<TTenant, TUser> : EntityMap<TTenant>
    where TTenant : AbpTenant<TUser>
    where TUser : AbpUser<TUser>
{
    /// <summary>
    /// Constructor.
    /// </summary>
    protected AbpTenantMap()
        : base("AbpTenants")
    {
        References(x => x.Edition).Column("EditionId").Nullable();
        Map(x => x.EditionId).ReadOnly();

        Map(x => x.TenancyName).Not.Nullable().Length(AbpTenantBase.MaxTenancyNameLength);
        Map(x => x.Name).Not.Nullable().Length(AbpTenantBase.MaxNameLength);
        Map(x => x.IsActive)
            .Not.Nullable();

        Map(x => x.ConnectionString).Length(AbpTenantBase.MaxConnectionStringLength);

        this.MapFullAudited();

        Polymorphism.Explicit();
    }
}