using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20131219
{
    [Migration(2013121901)]
    public class _01_AddPasswordResetCodeColumnToUsersTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Column("PasswordResetCode").OnTable("AbpUsers").AsString(32).Nullable();
        }
    }
}
