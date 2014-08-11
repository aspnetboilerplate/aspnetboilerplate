using Abp.Data.Migrations.FluentMigrator;
using FluentMigrator;

namespace Abp.Zero.DbMigrations.V20140323
{
    [Migration(2014032306)]
    public class _20140323_06_CreateAbpUserLoginsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpUserLogins")
                .WithIdAsInt64()
                .WithUserId()
                .WithColumn("LoginProvider").AsAnsiString(100).NotNullable()
                .WithColumn("ProviderKey").AsAnsiString(100).NotNullable();
        }
    }
}