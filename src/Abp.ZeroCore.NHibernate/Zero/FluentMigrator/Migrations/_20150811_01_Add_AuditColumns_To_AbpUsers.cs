using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015081101)]
    public class _20150811_01_Add_AuditColumns_To_AbpUsers : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("AbpUsers")
                .AddColumn("LastModificationTime").AsDateTime().Nullable()
                .AddColumn("LastModifierUserId").AsInt64().Nullable().ForeignKey("AbpUsers", "Id");
        }
    }
}