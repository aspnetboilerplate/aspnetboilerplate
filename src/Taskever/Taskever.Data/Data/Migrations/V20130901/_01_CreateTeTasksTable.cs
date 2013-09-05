using Abp.Modules.Core.Data.Migrations;
using FluentMigrator;

namespace Taskever.Data.Migrations.V20130901
{
    [Migration(2013090101)]
    public class _01_CreateTeTasksTable : Migration
    {
        public override void Up()
        {
            Create.Table("TeTasks")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("TenantId").AsInt32().NotNullable().ForeignKey("AbpTenants", "Id")
                .WithColumn("Title").AsString(200).NotNullable()
                .WithColumn("Description").AsString(2000).Nullable()
                .WithAuditColumns();
        }

        public override void Down()
        {
            Delete.Table("TeTasks");
        }
    }
}
