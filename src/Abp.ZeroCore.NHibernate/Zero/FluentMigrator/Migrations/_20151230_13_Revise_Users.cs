using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015123013)]
    public class _20151230_13_Revise_Users : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Column("Name").OnTable("AbpUsers").AsString(32).NotNullable();
            Alter.Column("Surname").OnTable("AbpUsers").AsString(32).NotNullable();
            Alter.Column("EmailAddress").OnTable("AbpUsers").AsString(256).NotNullable();
            Alter.Column("EmailConfirmationCode").OnTable("AbpUsers").AsString(128).Nullable();
            Alter.Column("PasswordResetCode").OnTable("AbpUsers").AsString(328).Nullable();
            Alter.Column("Password").OnTable("AbpUsers").AsString(128).NotNullable();

            Alter.Table("AbpUsers")
                .AddColumn("AuthenticationSource").AsString(64).Nullable();

            Create.Index("IX_IsDeleted_TenantId_EmailAddress")
                .OnTable("AbpUsers")
                .OnColumn("IsDeleted").Ascending()
                .OnColumn("TenantId").Ascending()
                .OnColumn("EmailAddress").Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_IsDeleted_TenantId_UserName")
                .OnTable("AbpUsers")
                .OnColumn("IsDeleted").Ascending()
                .OnColumn("TenantId").Ascending()
                .OnColumn("UserName").Ascending()
                .WithOptions().NonClustered();
        }
    }
}