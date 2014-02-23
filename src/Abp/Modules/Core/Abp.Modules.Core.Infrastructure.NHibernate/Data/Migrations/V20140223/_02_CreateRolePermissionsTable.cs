using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20140223
{
    [Migration(2014022301)]
    public class _01_CreateUserLoginsTable : ForwardOnlyMigration
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