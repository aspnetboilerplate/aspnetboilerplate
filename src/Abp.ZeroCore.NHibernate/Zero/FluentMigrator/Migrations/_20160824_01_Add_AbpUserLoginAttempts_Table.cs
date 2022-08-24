using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2016082401)]
    public class _20160824_01_Add_AbpUserLoginAttempts_Table : Migration
    {
        public override void Up()
        {
            Create.Table("AbpUserLoginAttempts")
                .WithTenantIdAsNullable()
                .WithColumn("TenancyName").AsString(64).Nullable()
                .WithNullableUserId()
                .WithColumn("UserNameOrEmailAddress").AsString(255).Nullable()
                .WithColumn("ClientIpAddress").AsString(64).Nullable()
                .WithColumn("ClientName").AsString(128).Nullable()
                .WithColumn("BrowserInfo").AsString(256).Nullable()
                .WithColumn("Result").AsByte().NotNullable()
                .WithCreationTimeColumn();
        }

        public override void Down()
        {
            Delete.Table("AbpUserLoginAttempts");
        }
    }
}