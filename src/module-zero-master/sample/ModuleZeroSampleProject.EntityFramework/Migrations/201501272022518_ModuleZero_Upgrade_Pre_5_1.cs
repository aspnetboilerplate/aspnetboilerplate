namespace ModuleZeroSampleProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleZero_Upgrade_Pre_5_1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbpRoles", "IsStatic", c => c.Boolean(nullable: false));
            AlterColumn("dbo.AbpUsers", "Name", c => c.String(nullable: false, maxLength: 32));
            AlterColumn("dbo.AbpUsers", "Surname", c => c.String(nullable: false, maxLength: 32));
            AlterColumn("dbo.AbpUsers", "Password", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.AbpUsers", "EmailAddress", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.AbpRoles", "Name", c => c.String(nullable: false, maxLength: 32));
            AlterColumn("dbo.AbpRoles", "DisplayName", c => c.String(nullable: false, maxLength: 64));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AbpRoles", "DisplayName", c => c.String());
            AlterColumn("dbo.AbpRoles", "Name", c => c.String());
            AlterColumn("dbo.AbpUsers", "EmailAddress", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.AbpUsers", "Password", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.AbpUsers", "Surname", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.AbpUsers", "Name", c => c.String(nullable: false, maxLength: 30));
            DropColumn("dbo.AbpRoles", "IsStatic");
        }
    }
}
