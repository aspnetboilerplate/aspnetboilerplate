using FluentMigrator;

namespace Taskever.Data.Migrations.V20130916
{
    [Migration(2013091602)]
    public class _02_CreateFriendshipTable : Migration
    {
        public override void Up()
        {
            Create.Table("TeFriendships") 
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("TenantId").AsInt32().NotNullable().ForeignKey("AbpTenants", "Id")
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("FriendUserId").AsInt32().NotNullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("Status").AsByte().NotNullable().WithDefaultValue(0); //FriendshipStatus.WaitingForApproval
        }

        public override void Down()
        {
            Delete.Column("Priority").FromTable("TeTasks");
        }
    }
}