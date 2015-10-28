namespace MyAbpZeroProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModuleZeroTo_0_6_9 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AbpUsers", "PasswordResetCode", c => c.String(maxLength: 328));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AbpUsers", "PasswordResetCode", c => c.String(maxLength: 128));
        }
    }
}
