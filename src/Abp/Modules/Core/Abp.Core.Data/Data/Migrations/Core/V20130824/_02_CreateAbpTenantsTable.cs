using FluentMigrator;
using Abp.Data.Migrations;
namespace Abp.Data.Migrations.Core.V20130824
{
    [Migration(2013082402)]
    public class _02_CreateAbpTenantsTable : Migration
    {
        public override void Up()
        {
            Create.Table("AbpTenants")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("CompanyName").AsString(100).NotNullable()
                .WithColumn("OwnerUserId").AsInt32().NotNullable().ForeignKey("AbpUsers", "Id")
                .WithAuditColumns();

            Insert.IntoTable("AbpTenants").Row(
                new
                    {
                        CompanyName = "Default",
                        OwnerUserId = 1,
                        CreatorUserId = 1
                    }
                );
        }

        public override void Down()
        {
            Delete.Table("AbpTenants");
        }
    }
}
