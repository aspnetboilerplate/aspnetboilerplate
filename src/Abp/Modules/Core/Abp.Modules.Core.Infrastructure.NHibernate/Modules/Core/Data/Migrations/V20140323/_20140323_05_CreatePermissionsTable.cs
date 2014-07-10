using Abp.Data.Migrations.FluentMigrator;
using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20140323
{
    [Migration(2014032305)]
    public class _20140323_05_CreatePermissionsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpPermissions")
                .WithIdAsInt64()
                .WithColumn("RoleId").AsInt32().Nullable().ForeignKey("AbpRoles", "Id")
                .WithNullableUserId()
                .WithColumn("Name").AsAnsiString(128).NotNullable()
                .WithColumn("IsGranted").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithCreationAuditColumns();
        }
    }
}