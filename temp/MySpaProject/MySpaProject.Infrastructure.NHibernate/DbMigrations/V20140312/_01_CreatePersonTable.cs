using FluentMigrator;

namespace MySpaProject.DbMigrations.V20140312
{
    [Migration(2014031201)]
    public class _01_CreatePersonTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("TsPeople")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().NotNullable()
                .WithColumn("Name").AsString(32).NotNullable();

            Insert.IntoTable("TsPeople")
                .Row(new { Name = "Douglas Adams" })
                .Row(new { Name = "Isaac Asimov" })
                .Row(new { Name = "George Orwell" })
                .Row(new { Name = "Thomas More" });
        }
    }
}
