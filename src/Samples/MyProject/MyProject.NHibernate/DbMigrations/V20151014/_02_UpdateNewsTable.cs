using FluentMigrator;

namespace MyProject.DbMigrations.V20151010
{
    [Migration(2015101002)]
    public class _02_UpdateNewsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("News")
                .WithColumn("Id").AsInt64().Identity().PrimaryKey().NotNullable() //.WithIdAsInt32() can be used here for shortcut.
                .WithColumn("Title").AsString(50).NotNullable()
                .WithColumn("Intro").AsString(800)
                .WithColumn("Content").AsString(8000)
                .WithColumn("CreationTime").AsDateTime()
                .WithColumn("CreatorUserId").AsInt64()
                .WithColumn("LastModificationTime").AsDateTime()
                .WithColumn("LastModifierUserId").AsInt64()
                .WithColumn("IsDeleted").AsBoolean()
                .WithColumn("DeleterUserId").AsInt64()
                .WithColumn("DeletionTime").AsDateTime();
           
        }
    }
}
