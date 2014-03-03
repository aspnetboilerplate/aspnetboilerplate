using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20130824
{
    [Migration(2013082404)]
    public class _04_CreateAbpUserRolesTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpUserRoles")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("RoleId").AsInt32().NotNullable().ForeignKey("AbpRoles", "Id")
                .WithCreationAuditColumns();

            Insert.IntoTable("AbpUserRoles").Row(
                new
                    {
                        UserId = 1,
                        RoleId = 1
                    }
                );
        }
    }
}