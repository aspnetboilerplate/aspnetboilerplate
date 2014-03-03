using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20130824
{
    [Migration(2013082403)]
    public class _03_CreateAbpRolesTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpRoles")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("DisplayName").AsString(100).NotNullable()
                .WithColumn("IsStatic").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("IsFrozen").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithAuditColumns();

            Insert.IntoTable("AbpRoles").Row(
                new
                    {
                        Name = "Admin",
                        DisplayName = "Admin"
                    }
                );
        }
    }
}
