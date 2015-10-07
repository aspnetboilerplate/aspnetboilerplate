using FluentMigrator;

namespace MyProject.DbMigrations.V20150811
{
    /// <summary>
    /// Defines a migration (database schema change).
    /// Creates Tasks table.
    /// See FluentMigrator's documentation for more information.
    /// </summary>
    [Migration(2015081102)]
    public class _02_CreateTasksTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("StsTasks")
                .WithColumn("Id").AsInt64().Identity().PrimaryKey().NotNullable() //.WithIdAsInt64() can be used here for shortcut.
                .WithColumn("AssignedPersonId").AsInt32().ForeignKey("StsPeople", "Id").Nullable()
                .WithColumn("Description").AsString(256).NotNullable()
                .WithColumn("State").AsByte().NotNullable().WithDefaultValue(1) //1: TaskState.New
                .WithColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime); //WithCreationTimeColumn() can be used here for shortcut.
        }
    }
}
