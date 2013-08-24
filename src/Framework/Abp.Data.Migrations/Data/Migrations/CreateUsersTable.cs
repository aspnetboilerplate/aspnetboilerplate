using FluentMigrator;

namespace Abp.Data.Migrations
{
    [Migration(2013082401)]
    public class CreateUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("Users")

                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()

                .WithColumn("EmailAddress").AsString(100).NotNullable()
                .WithColumn("Password").AsString(30).NotNullable();
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}
