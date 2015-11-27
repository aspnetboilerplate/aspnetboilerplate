namespace ModuleZeroSampleProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removed_Message_Class : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Answers", "Title");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Answers", "Title", c => c.String());
        }
    }
}
