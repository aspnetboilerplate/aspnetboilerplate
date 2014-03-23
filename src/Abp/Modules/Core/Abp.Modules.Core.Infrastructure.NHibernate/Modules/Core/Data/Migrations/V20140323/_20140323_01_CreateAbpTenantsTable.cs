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
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsAnsiString(128).NotNullable()
                .WithCreationTimeColumn();
        }
    }
}