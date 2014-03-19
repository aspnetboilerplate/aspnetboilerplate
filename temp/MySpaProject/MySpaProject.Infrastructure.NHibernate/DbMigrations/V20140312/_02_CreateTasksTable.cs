using FluentMigrator;

namespace MySpaProject.DbMigrations.V20140312
{
    [Migration(2014031202)]
    public class _02_CreateTasksTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("TsTasks")
                .WithColumn("Id").AsInt64().Identity().PrimaryKey().NotNullable()
                .WithColumn("AssignedPersonId").AsInt32().ForeignKey("TsPeople", "Id").Nullable()
                .WithColumn("Description").AsString(256).NotNullable()
                .WithColumn("State").AsByte().NotNullable().WithDefaultValue(1) //1: TaskState.New
                .WithColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        }
    }
}