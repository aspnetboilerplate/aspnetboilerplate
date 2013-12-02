using FluentMigrator;

namespace Taskever.Data.Migrations.V20131007
{
    [Migration(2013100702)]
    public class _02_CreateTeUserFollowedActivitiesTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("TeUserFollowedActivities")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("ActivityId").AsInt64().NotNullable().ForeignKey("TeActivities", "Id")
                .WithColumn("IsActor").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("IsRelated").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            Create.Index("IX_UserId_CreationTime")
                .OnTable("TeUserFollowedActivities")
                .OnColumn("UserId").Ascending()
                .OnColumn("CreationTime").Descending()
                .WithOptions().NonClustered();
        }
    }
}