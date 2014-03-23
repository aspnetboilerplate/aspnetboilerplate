using Abp.Data.Migrations.FluentMigrator;
using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20140323
{
    [Migration(2014032302)]
    public class _20140323_02_CreateAbpUsersTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpUsers")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithNullableTenantId()
                .WithColumn("UserName").AsString(32).NotNullable()
                .WithColumn("Name").AsString(30).NotNullable()
                .WithColumn("Surname").AsString(30).NotNullable()
                .WithColumn("EmailAddress").AsString(100).NotNullable()
                .WithColumn("IsEmailConfirmed").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("EmailConfirmationCode").AsString(16).Nullable()
                .WithColumn("PasswordResetCode").AsString(32).Nullable()
                .WithColumn("Password").AsString(100).NotNullable();

            Create.Index("AbpUsers_TenantId_UserName")
                .OnTable("AbpUsers")
                .OnColumn("TenantId").Ascending()
                .OnColumn("UserName").Ascending()
                .WithOptions().NonClustered();
            
            Create.Index("AbpUsers_UserName")
                .OnTable("AbpUsers")
                .OnColumn("UserName").Ascending()
                .WithOptions().Unique()
                .WithOptions().NonClustered();

            Create.Index("AbpUsers_TenantId_EmailAddress")
                .OnTable("AbpUsers")
                .OnColumn("TenantId").Ascending()
                .OnColumn("EmailAddress").Ascending()
                .WithOptions().NonClustered();

            Create.Index("AbpUsers_EmailAddress")
                .OnTable("AbpUsers")
                .OnColumn("EmailAddress").Ascending()
                .WithOptions().Unique()
                .WithOptions().NonClustered();

            Insert.IntoTable("AbpUsers").Row(
                new
                {
                    UserName = "admin",
                    Name = "System",
                    Surname = "Administrator",
                    EmailAddress = "admin@aspnetboilerplate.com",
                    Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
                });
        }
    }
}
