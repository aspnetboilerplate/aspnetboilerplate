using Abp.Data.Migrations.FluentMigrator;
using FluentMigrator;

namespace Taskever.Data.Migrations.V20130901
{
    [Migration(2013090102)]
    public class _02_CreateTeFriendshipTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("TeFriendships") 
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithUserId()
                .WithColumn("PairFriendshipId").AsInt32().Nullable().ForeignKey("TeFriendships", "Id") //TODO: Removed cascade update and delete! Test it!
                .WithUserId("FriendUserId")
                .WithColumn("FollowActivities").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CanAssignTask").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("LastVisitTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("Status").AsByte().NotNullable().WithDefaultValue(0); //FriendshipStatus.WaitingForApproval

            Create.Index("IX_User_Friend")
                .OnTable("TeFriendships")
                .OnColumn("UserId").Ascending()
                .OnColumn("FriendUserId").Ascending()
                .WithOptions().NonClustered();
        }
    }
}