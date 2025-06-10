using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015030101)]
    public class _20150301_01_Add_IsActive_To_AbpUsers_And_AbpTenants_Tables : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("AbpUsers").AddColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true);
            Alter.Table("AbpTenants").AddColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true);
        }
    }
}