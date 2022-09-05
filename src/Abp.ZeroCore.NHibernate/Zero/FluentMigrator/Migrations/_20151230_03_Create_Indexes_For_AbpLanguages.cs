using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015123003)]
    public class _20151230_03_Create_Indexes_For_AbpLanguages : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Index("IX_TenantId_Name")
                .OnTable("AbpLanguages")
                .OnColumn("TenantId").Ascending()
                .OnColumn("Name").Ascending()
                .WithOptions().NonClustered();
        }
    }
}