using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20130916
{
    [Migration(2013091601)]
    public class _01_AddIsFrozenColumnToRolesTable : Migration
    {
        public override void Up()
        {
            Alter.Table("AbpRoles")
                .AddColumn("IsFrozen").AsBoolean().NotNullable().WithDefaultValue(false);
        }

        public override void Down()
        {
            Delete.Column("IsFrozen").FromTable("AbpRoles");
        }
    }
}