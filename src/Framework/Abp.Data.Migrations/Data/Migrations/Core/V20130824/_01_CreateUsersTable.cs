using FluentMigrator;

namespace Abp.Data.Migrations.Core.V20130824
{
    [Migration(2013082401)]
    public class _01_CreateUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("EmailAddress").AsString(100).NotNullable()
                .WithColumn("Password").AsString(30).NotNullable();

            Insert.IntoTable("Users").Row(
                new
                    {
                        EmailAddress = "admin@aspnetboilerplate.com",
                        Password = "123"
                    }
                );
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}
