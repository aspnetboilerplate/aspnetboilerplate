using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20130824
{
    [Migration(2013082402)]
    public class _02_CreateAbpUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("AbpUsers")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("TenantId").AsInt32().NotNullable().ForeignKey("AbpTenants", "Id")
                .WithColumn("Name").AsString(30).NotNullable()
                .WithColumn("Surname").AsString(30).NotNullable()
                .WithColumn("EmailAddress").AsString(100).NotNullable()
                .WithColumn("Password").AsString(30).NotNullable()
                .WithColumn("IsTenantOwner").AsBoolean().NotNullable().WithDefaultValue(false);

            Insert.IntoTable("AbpUsers").Row(
                new
                    {
                        TenantId = 1,
                        Name = "System",
                        Surname = "Admin",
                        EmailAddress = "admin@aspnetboilerplate.com",
                        Password = "123"
                    }
                );
        }

        public override void Down()
        {
            Delete.Table("AbpUsers");
        }
    }
}
