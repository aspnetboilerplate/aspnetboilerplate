using Abp.Data.Migrations.FluentMigrator;
using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20140323
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
        }
    }
}