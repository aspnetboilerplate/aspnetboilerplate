using FluentMigrator;

namespace Taskever.Data.Migrations.V20131007
{
    [Migration(2013100701)]
    public class _01_CreateTeActivitiesTable : Migration
    {
        public override void Up()
        {
            Create.Table("TeActivities")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("ActorUserId").AsInt32().NotNullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("Action").AsInt32().NotNullable()
                .WithColumn("Data").AsString(4096)
                .WithColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        }

        public override void Down()
        {
            Delete.Table("TeActivities");
        }
    }
}
