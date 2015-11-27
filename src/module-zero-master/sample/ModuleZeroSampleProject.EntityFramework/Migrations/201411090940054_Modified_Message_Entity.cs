namespace ModuleZeroSampleProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modified_Message_Entity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Questions", "AnswerCount", c => c.Int(nullable: false));
            AddColumn("dbo.Questions", "ViewCount", c => c.Int(nullable: false));
            CreateIndex("dbo.Answers", "CreatorUserId");
            CreateIndex("dbo.Questions", "CreatorUserId");
            AddForeignKey("dbo.Answers", "CreatorUserId", "dbo.AbpUsers", "Id");
            AddForeignKey("dbo.Questions", "CreatorUserId", "dbo.AbpUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Questions", "CreatorUserId", "dbo.AbpUsers");
            DropForeignKey("dbo.Answers", "CreatorUserId", "dbo.AbpUsers");
            DropIndex("dbo.Questions", new[] { "CreatorUserId" });
            DropIndex("dbo.Answers", new[] { "CreatorUserId" });
            DropColumn("dbo.Questions", "ViewCount");
            DropColumn("dbo.Questions", "AnswerCount");
        }
    }
}
