using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015123004)]
    public class _20151230_04_Create_Indexes_For_AbpLanguages : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Index("IX_TenantId_LanguageName_Source_Key")
                .OnTable("AbpLanguageTexts")
                .OnColumn("TenantId").Ascending()
                .OnColumn("LanguageName").Ascending()
                .OnColumn("Source").Ascending()
                .OnColumn("Key").Ascending()
                .WithOptions().NonClustered();
        }
    }
}