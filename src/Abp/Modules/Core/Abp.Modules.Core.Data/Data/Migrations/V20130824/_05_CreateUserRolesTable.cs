using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20130824
{
    [Migration(2013082405)]
    public class _05_CreateAbpUserRolesTable : Migration
    {
        public override void Up()
        {
            Create.Table("AbpUserRoles")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("TenantId").AsInt32().NotNullable().ForeignKey("AbpTenants", "Id")
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("RoleId").AsInt32().NotNullable().ForeignKey("AbpRoles", "Id")
                .WithCreationAuditColumns();

            Insert.IntoTable("AbpUserRoles").Row(
                new
                    {
                        TenantId = 1,
                        UserId = 1,
                        RoleId = 1
                    }
                );
        }

        public override void Down()
        {
            Delete.Table("AbpUserRoles");
        }
    }
}