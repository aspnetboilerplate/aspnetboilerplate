using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
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