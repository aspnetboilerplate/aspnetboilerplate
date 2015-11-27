namespace ModuleZeroSampleProject.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class Abp_Upgrade_0_5_12 : DbMigration
    {
        public override void Up()
        {
            AlterTableAnnotations(
                "dbo.AbpRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 32),
                        DisplayName = c.String(nullable: false, maxLength: 64),
                        IsStatic = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Role_SoftDelete",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                });
            
            AddColumn("dbo.AbpRoles", "IsDefault", c => c.Boolean(nullable: false));
            AddColumn("dbo.AbpRoles", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.AbpRoles", "DeleterUserId", c => c.Long());
            AddColumn("dbo.AbpRoles", "DeletionTime", c => c.DateTime());
            AlterColumn("dbo.AbpSettings", "Name", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.AbpSettings", "Value", c => c.String(maxLength: 2000));
            CreateIndex("dbo.AbpRoles", "DeleterUserId");
            AddForeignKey("dbo.AbpRoles", "DeleterUserId", "dbo.AbpUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AbpRoles", "DeleterUserId", "dbo.AbpUsers");
            DropIndex("dbo.AbpRoles", new[] { "DeleterUserId" });
            AlterColumn("dbo.AbpSettings", "Value", c => c.String());
            AlterColumn("dbo.AbpSettings", "Name", c => c.String());
            DropColumn("dbo.AbpRoles", "DeletionTime");
            DropColumn("dbo.AbpRoles", "DeleterUserId");
            DropColumn("dbo.AbpRoles", "IsDeleted");
            DropColumn("dbo.AbpRoles", "IsDefault");
            AlterTableAnnotations(
                "dbo.AbpRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 32),
                        DisplayName = c.String(nullable: false, maxLength: 64),
                        IsStatic = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Role_SoftDelete",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                });
            
        }
    }
}
