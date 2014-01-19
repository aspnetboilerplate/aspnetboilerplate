using FluentMigrator;

namespace Taskever.Data.Migrations.V20131215
{
    [Migration(2013121501)]
    public class _01_ChangeIndexOnTeUserFollowedActivitiesTable : Migration
    {
        public override void Up()
        {
            Delete.Index("IX_UserId_CreationTime")
                .OnTable("TeUserFollowedActivities");

            Create.Index("IX_UserId_Id")
                .OnTable("TeUserFollowedActivities")
                .OnColumn("UserId").Ascending()
                .OnColumn("Id").Descending()
                .WithOptions().NonClustered();
        }

        public override void Down()
        {
            Create.Index("IX_UserId_CreationTime")
                .OnTable("TeUserFollowedActivities")
                .OnColumn("UserId").Ascending()
                .OnColumn("CreationTime").Descending()
                .WithOptions().NonClustered();
        }
    }
}