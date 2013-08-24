using FluentMigrator;

namespace Abp.Data.Migrations
{
    [Migration(2013082402)]
    public class CreateAccountsTable : Migration
    {
        public override void Up()
        {
            Create.Table("Accounts")
                
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                
                .WithColumn("CompanyName").AsString(100).NotNullable()
                .WithColumn("OwnerUserId").AsInt32().NotNullable().ForeignKey("Users", "Id")

                .WithColumn("CreationDate").AsDateTime().Nullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("CreatorUserId").AsInt32().NotNullable().ForeignKey("Users", "Id")
                
                .WithColumn("LastModificationDate").AsDateTime().Nullable()
                .WithColumn("LastModifierUserId").AsInt32().Nullable().ForeignKey("Users", "Id");
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}
