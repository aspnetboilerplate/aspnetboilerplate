using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2014032303)]
    public class _20140323_03_CreateAbpRolesTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpRoles")
                .WithIdAsInt32()
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("DisplayName").AsString(100).NotNullable()
                .WithAuditColumns();

            Insert.IntoTable("AbpRoles").Row(
                new
                    {
                        Name = "Admin",
                        DisplayName = "Admin"
                    }
                );
        }
    }
}
