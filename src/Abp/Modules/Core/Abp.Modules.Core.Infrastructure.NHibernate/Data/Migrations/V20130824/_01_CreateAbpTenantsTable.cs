using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20130824
{
    [Migration(2013082401)]
    public class _01_CreateAbpTenantsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpTenants")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("CompanyName").AsString(100).NotNullable()
                .WithColumn("Subdomain").AsString(50).NotNullable()
                .WithColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            Insert.IntoTable("AbpTenants").Row(
                new
                    {
                        CompanyName = "Default",
                        Subdomain = "default"
                    }
                );
        }

        //public override void Down()
        //{
        //    Delete.Table("AbpTenants");
        //}
    }
}
