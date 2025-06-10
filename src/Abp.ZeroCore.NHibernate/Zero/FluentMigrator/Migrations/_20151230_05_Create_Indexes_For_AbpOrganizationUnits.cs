using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015123005)]
    public class _20151230_05_Create_Indexes_For_AbpOrganizationUnits : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Index("IX_TenantId_ParentId")
                .OnTable("AbpOrganizationUnits")
                .OnColumn("TenantId").Ascending()
                .OnColumn("ParentId").Ascending()
                .WithOptions().NonClustered();
        }
    }
}
