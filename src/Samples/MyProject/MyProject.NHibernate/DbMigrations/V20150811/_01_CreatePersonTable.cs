using FluentMigrator;

namespace MyProject.DbMigrations.V20150811
{
    /// <summary>
    /// Defines a migration (database schema change).
    /// Creates Person table and inserts a few person.
    /// See FluentMigrator's documentation for more information.
    /// </summary>
    [Migration(2015081101)]
    public class _01_CreatePersonTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("StsPeople")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().NotNullable() //.WithIdAsInt32() can be used here for shortcut.
                .WithColumn("Name").AsString(32).NotNullable();

            Insert.IntoTable("StsPeople")
                .Row(new { Name = "Douglas Adams" })
                .Row(new { Name = "Isaac Asimov" })
                .Row(new { Name = "George Orwell" })
                .Row(new { Name = "Thomas More" });
        }
    }
}
