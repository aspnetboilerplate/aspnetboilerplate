using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015110302)]
    public class _20151103_02_Create_AbpLanguageTexts_Table : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpLanguageTexts")
                .WithIdAsInt64()
                .WithTenantIdAsNullable()
                .WithColumn("LanguageName").AsString(10).NotNullable()
                .WithColumn("Source").AsString(128).NotNullable()
                .WithColumn("Key").AsString(256).NotNullable()
                .WithColumn("Value").AsString(64 * 1024 * 1024).NotNullable() //64KB
                .WithAuditColumns();
        }
    }
}