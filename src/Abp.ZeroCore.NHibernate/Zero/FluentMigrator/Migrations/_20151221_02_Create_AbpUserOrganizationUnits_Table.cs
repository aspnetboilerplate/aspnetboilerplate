using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015122102)]
    public class _20151221_02_Create_AbpUserOrganizationUnits_Table : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpUserOrganizationUnits")
                .WithIdAsInt64()
                .WithTenantIdAsNullable()
                .WithUserId()
                .WithColumn("OrganizationUnitId").AsInt64().NotNullable().ForeignKey("AbpOrganizationUnits", "Id")
                .WithCreationAuditColumns();

            Create.Index("IX_TenantId_UserId")
                .OnTable("AbpUserOrganizationUnits")
                .OnColumn("TenantId").Ascending()
                .OnColumn("UserId").Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TenantId_OrganizationUnitId")
                .OnTable("AbpUserOrganizationUnits")
                .OnColumn("TenantId").Ascending()
                .OnColumn("OrganizationUnitId").Ascending()
                .WithOptions().NonClustered();
        }
    }
}