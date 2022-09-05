using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015123011)]
    public class _20151230_11_Create_Indexes_For_AbpUserOrganizationUnits : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Index("IX_OrganizationUnitId")
                .OnTable("AbpUserOrganizationUnits")
                .OnColumn("OrganizationUnitId").Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_UserId")
                .OnTable("AbpUserOrganizationUnits")
                .OnColumn("UserId").Ascending()
                .WithOptions().NonClustered();
        }
    }
}