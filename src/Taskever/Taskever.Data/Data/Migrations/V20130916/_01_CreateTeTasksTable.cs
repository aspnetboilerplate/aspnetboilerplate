using FluentMigrator;

namespace Taskever.Data.Migrations.V20130916
{
    [Migration(2013091601)]
    public class _01_AddPriorityToTasksTable : Migration
    {
        public override void Up()
        {
            Alter.Table("TeTasks")
                .AddColumn("Priority").AsByte().NotNullable().WithDefaultValue(3); //TaskPriority.Normal
        }

        public override void Down()
        {
            Delete.Column("Priority").FromTable("TeTasks");
        }
    }
}
