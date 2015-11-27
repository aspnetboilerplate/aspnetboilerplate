namespace ModuleZeroSampleProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rename_IsSuspended_To_IsActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbpUsers", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.AbpTenants", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AbpTenants", "IsActive");
            DropColumn("dbo.AbpUsers", "IsActive");
        }
    }
}
