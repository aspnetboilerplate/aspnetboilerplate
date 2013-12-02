using FluentMigrator;

namespace Taskever.Data.Migrations.V20131007
{
    [Migration(2013100701)]
    public class _01_CreateTeActivitiesTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("TeActivities")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("ActivityType").AsInt32().NotNullable()
                .WithColumn("CreatorUserId").AsInt32().Nullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("AssignedUserId").AsInt32().Nullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("TaskId").AsInt32().Nullable().ForeignKey("TeTasks", "Id")
                .WithColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        }
    }
}
