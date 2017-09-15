using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2014032301)]
    public class _20140323_01_CreateAbpTenantsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpTenants")
                .WithIdAsInt32()
                .WithColumn("TenancyName").AsString(32).NotNullable()
                .WithColumn("Name").AsString(128).NotNullable()
                .WithCreationTimeColumn();

            Create.Index("AbpTenants_TenancyName")
                .OnTable("AbpTenants")
                .OnColumn("TenancyName").Ascending()
                .WithOptions().Unique()
                .WithOptions().NonClustered();

            //Default tenant
            Insert.IntoTable("AbpTenants").Row(
                new
                {
                    TenancyName = "Default", //Reserved TenancyName
                    Name = "Default"
                });
        }
    }
}