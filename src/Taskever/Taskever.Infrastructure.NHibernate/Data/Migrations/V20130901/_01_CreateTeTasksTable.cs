using Abp.Modules.Core.Data.Migrations;
using FluentMigrator;

namespace Taskever.Data.Migrations.V20130901
{
    [Migration(2013090101)]
    public class _01_CreateTeTasksTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("TeTasks")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Title").AsString(200).NotNullable()
                .WithColumn("Description").AsString(2000).Nullable()
                .WithColumn("Priority").AsByte().NotNullable().WithDefaultValue(3) //TaskPriority.Normal
                .WithColumn("Privacy").AsByte().NotNullable().WithDefaultValue(2) //TaskPrivacy.Protected
                .WithColumn("AssignedUserId").AsInt32().Nullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("State").AsByte().NotNullable().WithDefaultValue(1) //TaskState.New
                .WithAuditColumns();
        }
    }
}
