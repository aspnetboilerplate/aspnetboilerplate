using Abp.Modules.Core.Entities;
using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20130915
{
    [Migration(2013091503)]
    public class _03_CreateUserRolesTable : Migration
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