using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015123012)]
    public class _20151230_12_Create_Indexes_For_AbpUserRoles : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Index("IX_RoleId")
                .OnTable("AbpUserRoles")
                .OnColumn("RoleId").Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_UserId_RoleId")
                .OnTable("AbpUserRoles")
                .OnColumn("UserId").Ascending()
                .OnColumn("RoleId").Ascending()
                .WithOptions().NonClustered();
        }
    }
}