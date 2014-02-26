using FluentMigrator;

namespace Taskever.Data.Migrations.V20131215
{
    [Migration(2013121502)]
    public class _02_AddProfileImageColumnToAbpUsers : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Column("ProfileImage").OnTable("AbpUsers").AsString(100).Nullable();
        }
    }
}