using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015123006)]
    public class _20151230_06_Create_Indexes_For_AbpPermissions : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Index("IX_RoleId_Name")
                .OnTable("AbpPermissions")
                .OnColumn("RoleId").Ascending()
                .OnColumn("Name").Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_UserId_Name")
                .OnTable("AbpPermissions")
                .OnColumn("UserId").Ascending()
                .OnColumn("Name").Ascending()
                .WithOptions().NonClustered();
        }
    }
}