using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20131215
{
    [Migration(2013121501)]
    public class _01_AddEmailConfirmationColumnsToUsersTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Column("IsEmailConfirmed").OnTable("AbpUsers").AsBoolean().NotNullable().WithDefaultValue(false);
            Create.Column("EmailConfirmationCode").OnTable("AbpUsers").AsString(16).NotNullable().WithDefaultValue("");
        }
    }
}
