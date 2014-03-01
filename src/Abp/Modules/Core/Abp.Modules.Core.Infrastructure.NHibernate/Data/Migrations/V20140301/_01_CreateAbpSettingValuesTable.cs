using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20140301
{
    [Migration(2014030101)]
    public class _01_CreateAbpSettingValuesTable : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("AbpSettingValues")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("UserId").AsInt32().Nullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("Name").AsAnsiString(128).NotNullable()
                .WithColumn("Value").AsString().NotNullable()
                .WithAuditColumns();
        }
    }
}