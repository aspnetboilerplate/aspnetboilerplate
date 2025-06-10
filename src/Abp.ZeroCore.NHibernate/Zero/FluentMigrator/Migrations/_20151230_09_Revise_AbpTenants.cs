using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015123009)]
    public class _20151230_09_Revise_AbpTenants : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Column("TenancyName").OnTable("AbpTenants").AsString(64).NotNullable();

            Alter.Table("AbpTenants")
                .AddColumn("EditionId").AsInt32().Nullable().ForeignKey("AbpEditions", "Id")
                .AddCreatorUserIdColumn()
                .AddModificationAuditColumns()
                .AddDeletionAuditColumns();

            Create.Index("IX_EditionId")
                .OnTable("AbpTenants")
                .OnColumn("EditionId").Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_IsDeleted_TenancyName")
                .OnTable("AbpTenants")
                .OnColumn("IsDeleted").Ascending()
                .OnColumn("TenancyName").Ascending()
                .WithOptions().NonClustered();
        }
    }
}