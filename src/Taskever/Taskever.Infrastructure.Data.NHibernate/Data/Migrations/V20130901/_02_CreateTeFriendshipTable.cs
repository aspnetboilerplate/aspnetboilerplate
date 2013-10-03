using FluentMigrator;

namespace Taskever.Data.Migrations.V20130916
{
    [Migration(2013090102)]
    public class _02_CreateTeFriendshipTable : Migration
    {
        public override void Up()
        {
            Create.Table("TeFriendships") 
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("TenantId").AsInt32().NotNullable().ForeignKey("AbpTenants", "Id")
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("FriendUserId").AsInt32().NotNullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("CanAssignTask").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("Status").AsByte().NotNullable().WithDefaultValue(0); //FriendshipStatus.WaitingForApproval
        }

        public override void Down()
        {
            Delete.Column("Priority").FromTable("TeTasks");
        }
    }
}