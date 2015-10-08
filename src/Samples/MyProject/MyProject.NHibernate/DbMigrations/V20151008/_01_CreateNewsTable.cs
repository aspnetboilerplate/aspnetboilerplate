using FluentMigrator;

namespace MyProject.DbMigrations.V20151008
{
    [Migration(2015100801)]
    public class _01_CreateNewsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("News")
                .WithColumn("Id").AsInt64().Identity().PrimaryKey().NotNullable() //.WithIdAsInt32() can be used here for shortcut.
                .WithColumn("Title").AsString(50).NotNullable()
                .WithColumn("Intro").AsString(800)
                .WithColumn("Content").AsString(8000);
           
        }
    }
}
