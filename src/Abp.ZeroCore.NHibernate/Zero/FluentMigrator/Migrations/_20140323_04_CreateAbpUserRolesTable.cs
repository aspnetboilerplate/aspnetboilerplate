using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2014032304)]
    public class _20140323_04_CreateAbpUserRolesTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpUserRoles")
                .WithIdAsInt64()
                .WithUserId()
                .WithColumn("RoleId").AsInt32().NotNullable().ForeignKey("AbpRoles", "Id")
                .WithCreationAuditColumns();

            Insert.IntoTable("AbpUserRoles").Row(
                new
                    {
                        UserId = 1,
                        RoleId = 1
                    }
                );
        }
    }
}