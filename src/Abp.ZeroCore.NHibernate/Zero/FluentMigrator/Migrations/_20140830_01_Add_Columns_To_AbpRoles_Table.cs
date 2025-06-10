using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2014083001)]
    public class _20140830_01_Add_Columns_To_AbpRoles_Table : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("AbpRoles")
                .AddTenantIdColumnAsNullable();
        }
    }
}