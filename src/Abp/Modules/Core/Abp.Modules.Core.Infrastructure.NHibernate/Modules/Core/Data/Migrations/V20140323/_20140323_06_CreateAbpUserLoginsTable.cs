using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20140323
{
    [Migration(2014032306)]
    public class _20140323_06_CreateAbpUserLoginsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpUserLogins")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("LoginProvider").AsAnsiString(100).NotNullable()
                .WithColumn("ProviderKey").AsAnsiString(100).NotNullable();
        }
    }
}