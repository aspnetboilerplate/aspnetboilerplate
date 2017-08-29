using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2014032307)]
    public class _20140323_07_CreateAbpSettingsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpSettings")
                .WithIdAsInt64()
                .WithTenantIdAsNullable()
                .WithNullableUserId()
                .WithColumn("Name").AsAnsiString(128).NotNullable()
                .WithColumn("Value").AsString().NotNullable()
                .WithAuditColumns();
        }
    }
}