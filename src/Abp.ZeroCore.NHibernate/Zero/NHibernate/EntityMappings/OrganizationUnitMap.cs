using Abp.NHibernate.EntityMappings;
using Abp.Organizations;

namespace Abp.Zero.NHibernate.EntityMappings;

public class OrganizationUnitMap : EntityMap<OrganizationUnit, long>
{
    public OrganizationUnitMap()
        : base("AbpOrganizationUnits")
    {
        Map(x => x.TenantId);
        References(x => x.Parent)
            .Column(nameof(OrganizationUnit.ParentId))
            .Nullable()
            .ReadOnly();
        Map(x => x.ParentId);
        Map(x => x.Code)
            .Length(OrganizationUnit.MaxCodeLength)
            .Not.Nullable();
        Map(x => x.DisplayName)
            .Length(OrganizationUnit.MaxDisplayNameLength)
            .Not.Nullable();
        HasMany(x => x.Children)
            .KeyColumn(nameof(OrganizationUnit.ParentId));

        this.MapFullAudited();
    }
}