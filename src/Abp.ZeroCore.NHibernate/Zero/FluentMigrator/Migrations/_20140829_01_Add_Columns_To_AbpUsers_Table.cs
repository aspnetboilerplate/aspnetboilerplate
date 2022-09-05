using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2014082901)]
    public class _20140829_01_Add_Columns_To_AbpUsers_Table : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("AbpUsers")
                .AddCreationAuditColumns()
                .AddColumn("LastLoginTime").AsDateTime().Nullable();
        }
    }
}