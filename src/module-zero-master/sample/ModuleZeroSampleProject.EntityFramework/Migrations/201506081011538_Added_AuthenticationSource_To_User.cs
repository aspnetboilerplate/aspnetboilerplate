namespace ModuleZeroSampleProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_AuthenticationSource_To_User : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbpUsers", "AuthenticationSource", c => c.String(maxLength: 64));
            AlterColumn("dbo.AbpUserLogins", "LoginProvider", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.AbpUserLogins", "ProviderKey", c => c.String(nullable: false, maxLength: 256));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AbpUserLogins", "ProviderKey", c => c.String());
            AlterColumn("dbo.AbpUserLogins", "LoginProvider", c => c.String());
            DropColumn("dbo.AbpUsers", "AuthenticationSource");
        }
    }
}
