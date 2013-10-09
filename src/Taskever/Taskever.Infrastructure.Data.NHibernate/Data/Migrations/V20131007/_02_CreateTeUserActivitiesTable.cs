using FluentMigrator;

namespace Taskever.Data.Migrations.V20131007
{
    [Migration(2013100702)]
    public class _02_CreateTeUserActivitiesTable : Migration
    {
        public override void Up()
        {
            Create.Table("TeUserActivities")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("ActivityId").AsInt64().NotNullable().ForeignKey("TeActivities", "Id");
        }

        public override void Down()
        {
            Delete.Table("TeUserActivities");
        }
    }
}