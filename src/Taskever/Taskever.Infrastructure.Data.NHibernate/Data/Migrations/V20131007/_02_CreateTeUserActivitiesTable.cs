using FluentMigrator;

namespace Taskever.Data.Migrations.V20131007
{
    [Migration(2013100702)]
    public class _02_CreateTeUserFollowedActivitiesTable : Migration
    {
        public override void Up()
        {
            Create.Table("TeUserFollowedActivities")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("ActivityId").AsInt64().NotNullable().ForeignKey("TeActivities", "Id")
                .WithColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            Create.Index("IX_UserId_CreationTime")
                .OnTable("TeUserFallowedActivities")
                .OnColumn("UserId").Ascending().OnColumn("CreationTime").Descending();
        }

        public override void Down()
        {
            Delete.Table("TeUserFallowedActivities");
        }
    }
}