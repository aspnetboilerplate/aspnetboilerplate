using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015123008)]
    public class _20151230_08_Revise_AbpSettings : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Column("Name").OnTable("AbpSettings").AsString(256).NotNullable();
            Alter.Column("Value").OnTable("AbpSettings").AsString(2000).Nullable();

            Create.Index("IX_UserId_Name")
                .OnTable("AbpSettings")
                .OnColumn("UserId").Ascending()
                .OnColumn("Name").Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_TenantId_Name")
                .OnTable("AbpSettings")
                .OnColumn("TenantId").Ascending()
                .OnColumn("Name").Ascending()
                .WithOptions().NonClustered();
        }
    }
}