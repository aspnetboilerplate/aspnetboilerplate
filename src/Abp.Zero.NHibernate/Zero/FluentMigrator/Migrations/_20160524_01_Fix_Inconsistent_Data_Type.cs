using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2016052401)]
    public class _20160524_01_Fix_Inconsistent_Data_Type : Migration
    {
        public override void Up()
        {
            this.Delete.PrimaryKey("PK_AbpBackgroundJobs")
               .FromTable("AbpBackgroundJobs");
            this.Alter.Column("Id").OnTable("AbpBackgroundJobs")
                .AsInt64().NotNullable();
            this.Create.PrimaryKey("PK_AbpBackgroundJobs")
                .OnTable("AbpBackgroundJobs")
                .Column("Id");

            this.Delete.Index("IX_RoleId_Name")
                .OnTable("AbpPermissions");
            this.Delete.Index("IX_UserId_Name")
                .OnTable("AbpPermissions");
            this.Alter.Column("Name")
                .OnTable("AbpPermissions")
                .AsString(128)
                .NotNullable();

            this.Create.Index("IX_RoleId_Name")
                .OnTable("AbpPermissions")
                .OnColumn("RoleId").Ascending()
                .OnColumn("Name").Ascending()
                .WithOptions().NonClustered();
            this.Create.Index("IX_UserId_Name")
                .OnTable("AbpPermissions")
                .OnColumn("UserId").Ascending()
                .OnColumn("Name").Ascending()
                .WithOptions().NonClustered();
        }

        public override void Down()
        {
            this.Delete.Index("IX_RoleId_Name")
                .OnTable("AbpPermissions");
            this.Delete.Index("IX_UserId_Name")
                .OnTable("AbpPermissions");
            this.Alter.Column("Name")
                .OnTable("AbpPermissions")
                .AsAnsiString(128)
                .NotNullable();
            this.Create.Index("IX_RoleId_Name")
                .OnTable("AbpPermissions")
                .OnColumn("RoleId").Ascending()
                .OnColumn("Name").Ascending()
                .WithOptions().NonClustered();
            this.Create.Index("IX_UserId_Name")
                .OnTable("AbpPermissions")
                .OnColumn("UserId").Ascending()
                .OnColumn("Name").Ascending()
                .WithOptions().NonClustered();

            this.Delete.PrimaryKey("PK_AbpBackgroundJobs")
                .FromTable("AbpBackgroundJobs");
            this.Alter.Column("Id").OnTable("AbpBackgroundJobs")
                .AsInt32().NotNullable().PrimaryKey("PK_AbpBackgroundJobs");
            this.Create.PrimaryKey("PK_AbpBackgroundJobs")
                .OnTable("AbpBackgroundJobs")
                .Column("Id");
        }
    }
}
