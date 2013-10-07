using Abp.Modules.Core.Data.Migrations;
using FluentMigrator;

namespace Taskever.Data.Migrations.V20131007
{
    [Migration(2013100701)]
    public class _01_CreateTeEventHistoriesTable : Migration
    {
        public override void Up()
        {
            Create.Table("TeEventHistories")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("HistoryType").AsInt32().NotNullable()
                .WithColumn("HistoryVersion").AsInt16().NotNullable()
                .WithColumn("HistoryText").AsString(1024)
                .WithCreationAuditColumns();
        }

        public override void Down()
        {
            Delete.Table("TeEventHistories");
        }
    }
}
