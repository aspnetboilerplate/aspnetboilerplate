using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20130824
{
    [Migration(2013082401)]
    public class _01_CreateAbpUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("AbpUsers")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString(30).NotNullable()
                .WithColumn("Surname").AsString(30).NotNullable()
                .WithColumn("EmailAddress").AsString(100).NotNullable()
                .WithColumn("Password").AsString(30).NotNullable();

            Insert.IntoTable("AbpUsers").Row(
                new
                    {
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
