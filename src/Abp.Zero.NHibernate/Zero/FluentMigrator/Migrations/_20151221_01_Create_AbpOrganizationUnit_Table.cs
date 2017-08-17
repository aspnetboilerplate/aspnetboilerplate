using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015122101)]
    public class _20151221_01_Create_AbpOrganizationUnit_Table : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpOrganizationUnits")
                .WithIdAsInt64()
                .WithTenantIdAsNullable()
                .WithColumn("ParentId").AsInt64().Nullable().ForeignKey("AbpOrganizationUnits", "Id")
                .WithColumn("Code").AsString(128).NotNullable()
                .WithColumn("DisplayName").AsString(128).NotNullable()
                .WithFullAuditColumns();

            Create.Index("IX_TenantId_Code")
                .OnTable("AbpOrganizationUnits")
                .OnColumn("TenantId").Ascending()
                .OnColumn("Code").Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_ParentId_Code")
                .OnTable("AbpOrganizationUnits")
                .OnColumn("ParentId").Ascending()
                .OnColumn("Code").Ascending()
                .WithOptions().NonClustered();
        }
    }
}