using FluentMigrator;

namespace Taskever.Data.Migrations.V20130922
{
    [Migration(2013092201)]
    public class _01_AddNewcolumnsToTasksTable : Migration
    {
        public override void Up()
        {
            Alter.Table("TeTasks")
                .AddColumn("State").AsByte().NotNullable().WithDefaultValue(1) //TaskState.New
                .AddColumn("AssignedUserId").AsInt32().Nullable().ForeignKey("AbpUsers", "Id");
        }

        public override void Down()
        {
            Delete.Column("State").FromTable("TeTasks");
            Delete.Column("AssignedUserId").FromTable("TeTasks");
        }
    }
}
