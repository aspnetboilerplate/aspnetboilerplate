using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2014032302)]
    public class _20140323_02_CreateAbpUsersTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpUsers")
                .WithIdAsInt64()
                .WithTenantIdAsNullable()
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
                .WithOptions().Unique()
                .WithOptions().NonClustered();

            Create.Index("AbpUsers_TenantId_EmailAddress")
                .OnTable("AbpUsers")
                .OnColumn("TenantId").Ascending()
                .OnColumn("EmailAddress").Ascending()
                .WithOptions().Unique()
                .WithOptions().NonClustered();
        }
    }
}
