using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015081103)]
    public class _20150811_03_Add_Columns_To_AbpRoles : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("AbpRoles")
                .AddColumn("IsStatic").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("IsDefault").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("DeleterUserId").AsInt64().Nullable().ForeignKey("AbpUsers", "Id")
                .AddColumn("DeletionTime").AsDateTime().Nullable();
        }
    }
}