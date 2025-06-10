using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015123010)]
    public class _20151230_10_Revise_AbpUserLogins : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Column("LoginProvider").OnTable("AbpUserLogins").AsString(128).NotNullable();
            Alter.Column("ProviderKey").OnTable("AbpUserLogins").AsString(256).NotNullable();

            Create.Index("IX_UserId_LoginProvider")
                .OnTable("AbpUserLogins")
                .OnColumn("UserId").Ascending()
                .OnColumn("LoginProvider").Ascending()
                .WithOptions().NonClustered();
        }
    }
}