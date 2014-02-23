using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20140223
{
    [Migration(2014022301)]
    public class _01_CreateUserLoginsTable : ForwardOnlyMigration
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