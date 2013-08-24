using FluentMigrator;

namespace Abp.Data.Migrations.V20130824
{
    [Migration(2013082402)]
    public class _02_CreateAccountsTable : Migration
    {
        public override void Up()
        {
            Create.Table("Accounts")
                
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                
                .WithColumn("CompanyName").AsString(100).NotNullable()
                .WithColumn("OwnerUserId").AsInt32().NotNullable().ForeignKey("Users", "Id")

                .WithColumn("CreationDate").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("CreatorUserId").AsInt32().Nullable().ForeignKey("Users", "Id")
                
                .WithColumn("LastModificationDate").AsDateTime().Nullable()
                .WithColumn("LastModifierUserId").AsInt32().Nullable().ForeignKey("Users", "Id");
           
            Insert.IntoTable("Accounts").Row(
                new
                    {
                        CompanyName = "Default",
                        OwnerUserId = 1,
                        CreatorUserId = 1
                    }
                );
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}
