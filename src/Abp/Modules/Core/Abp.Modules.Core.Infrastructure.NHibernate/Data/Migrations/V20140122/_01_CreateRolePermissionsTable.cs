using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20140122
{
    [Migration(2014012202)]
    public class _01_CreateRolePermissionsTable : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("AbpRolePermissions")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("RoleId").AsInt32().NotNullable().ForeignKey("AbpRoles", "Id")
                .WithColumn("PermissionName").AsString(100).NotNullable()
                .WithColumn("IsGranted").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithCreationAuditColumns();
        }
    }
}