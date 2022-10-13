using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015081201)]
    public class _20150812_01_Add_Discriminator_To_AbpPermissions : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("AbpPermissions")
                .AddColumn("Discriminator").AsString(128).NotNullable();
        }
    }
}