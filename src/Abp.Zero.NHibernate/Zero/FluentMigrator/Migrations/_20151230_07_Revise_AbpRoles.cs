using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015123007)]
    public class _20151230_07_Revise_AbpRoles : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Column("Name").OnTable("AbpRoles").AsString(32).NotNullable();
            Alter.Column("DisplayName").OnTable("AbpRoles").AsString(64).NotNullable();

            Create.Index("IX_IsDeleted_TenantId_Name")
                .OnTable("AbpRoles")
                .OnColumn("IsDeleted").Ascending()
                .OnColumn("TenantId").Ascending()
                .OnColumn("Name").Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TenantId_Name")
                .OnTable("AbpRoles")
                .OnColumn("TenantId").Ascending()
                .OnColumn("Name").Ascending()
                .WithOptions().NonClustered();
        }
    }
}