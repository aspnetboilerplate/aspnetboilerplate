using FluentMigrator;

namespace MyProject.DbMigrations.V20151010
{
    [Migration(2015101003)]
    public class _03_UpdateNewsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("News")
                .WithColumn("Id").AsInt64().Identity().PrimaryKey().NotNullable() //.WithIdAsInt32() can be used here for shortcut.
                .WithColumn("Title").AsString(50).NotNullable()
                .WithColumn("Intro").AsString(800)
                .WithColumn("Content").AsString(8000)
                .WithColumn("CreationTime").AsDateTime()
                .WithColumn("CreatorUserId").AsInt64().Nullable()
                .WithColumn("LastModificationTime").AsDateTime()
                .WithColumn("LastModifierUserId").AsInt64().Nullable()
                .WithColumn("IsDeleted").AsBoolean()
                .WithColumn("DeleterUserId").AsInt64().Nullable()
                .WithColumn("DeletionTime").AsDateTime();
           
        }
    }
}
