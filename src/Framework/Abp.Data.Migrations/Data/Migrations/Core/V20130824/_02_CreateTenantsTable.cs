using FluentMigrator;
using Abp.Data.Migrations;
namespace Abp.Data.Migrations.Core.V20130824
{
    [Migration(2013082402)]
    public class _02_CreateTenantsTable : Migration
    {
        public override void Up()
        {
            Create.Table("Tenants")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("CompanyName").AsString(100).NotNullable()
                .WithColumn("OwnerUserId").AsInt32().NotNullable().ForeignKey("Users", "Id")
                .WithAuditColumns();

            Insert.IntoTable("Tenants").Row(
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
            Delete.Table("Tenants");
        }
    }
}
