using Abp.Auditing;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Localization;
using Abp.Organizations;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations;

[Migration(2022_10_24_02)]
public class _2022_10_24_02_Fix_Inconsistent_Data_Type : Migration
{
    public override void Up()
    {
        #region AbpAuditLogs

        Delete.Index("IX_TenantId_ExecutionTime").OnTable("AbpAuditLogs");
        Delete.Index("IX_UserId_ExecutionTime").OnTable("AbpAuditLogs");

        Alter.Column("ExecutionTime").OnTable("AbpAuditLogs").AsDateTime2().NotNullable();

        Create.Index("IX_TenantId_ExecutionTime").OnTable("AbpAuditLogs")
            .OnColumn("TenantId").Ascending()
            .OnColumn("ExecutionTime").Ascending()
            .WithOptions().NonClustered();
        Create.Index("IX_UserId_ExecutionTime").OnTable("AbpAuditLogs")
            .OnColumn("UserId").Ascending()
            .OnColumn("ExecutionTime").Ascending()
            .WithOptions().NonClustered();

        Alter.Column("BrowserInfo").OnTable("AbpAuditLogs").AsString(AuditLog.MaxBrowserInfoLength).Nullable();
        #endregion
        #region AbpBackgroundJobs
        Delete.Index("IX_IsAbandoned_NextTryTime").OnTable("AbpBackgroundJobs");
        Delete.DefaultConstraint().OnTable("AbpBackgroundJobs").OnColumn("TryCount");
        Delete.DefaultConstraint().OnTable("AbpBackgroundJobs").OnColumn("IsAbandoned");
        Delete.DefaultConstraint().OnTable("AbpBackgroundJobs").OnColumn("Priority");

        Alter.Column("IsAbandoned").OnTable("AbpBackgroundJobs").AsBoolean().NotNullable();

        Alter.Column("NextTryTime").OnTable("AbpBackgroundJobs").AsDateTime2().NotNullable();

        Create.Index("IX_AbpBackgroundJobs_IsAbandoned_NextTryTime").OnTable("AbpBackgroundJobs")
            .OnColumn("IsAbandoned").Ascending()
            .OnColumn("NextTryTime").Ascending()
            .WithOptions().NonClustered();

        Delete.DefaultConstraint().OnTable("AbpBackgroundJobs").OnColumn("CreationTime");
        Alter.Column("CreationTime").OnTable("AbpBackgroundJobs").AsDateTime2().NotNullable();
        Alter.Column("LastTryTime").OnTable("AbpBackgroundJobs").AsDateTime2().Nullable();
        #endregion
        #region AbpEditions
        Delete.DefaultConstraint().OnTable("AbpEditions").OnColumn("CreationTime");
        Delete.DefaultConstraint().OnTable("AbpEditions").OnColumn("IsDeleted");
        Alter.Column("CreationTime").OnTable("AbpEditions").AsDateTime2().NotNullable();
        Alter.Column("LastModificationTime").OnTable("AbpEditions").AsDateTime2().Nullable();
        Alter.Column("DeletionTime").OnTable("AbpEditions").AsDateTime2().Nullable();
        #endregion
        #region AbpFeatures
        Delete.DefaultConstraint().OnTable("AbpFeatures").OnColumn("CreationTime");
        Alter.Column("CreationTime").OnTable("AbpFeatures").AsDateTime2().NotNullable();

        Delete.Index("IX_Discriminator_EditionId_Name").OnTable("AbpFeatures");
        Delete.Index("IX_Discriminator_TenantId_Name").OnTable("AbpFeatures");

        Alter.Column("Discriminator").OnTable("AbpFeatures").AsString(int.MaxValue).NotNullable();
        #endregion
        #region AbpLanguages
        Delete.Index("IX_TenantId_Name").OnTable("AbpLanguages");
        Alter.Column("Name").OnTable("AbpLanguages").AsString(ApplicationLanguage.MaxNameLength).NotNullable();
        Create.Index().OnTable("AbpLanguages")
            .OnColumn("TenantId").Ascending()
            .OnColumn("Name").Ascending()
            .WithOptions().NonClustered();

        Delete.DefaultConstraint().OnTable("AbpLanguages").OnColumn("CreationTime");
        Delete.DefaultConstraint().OnTable("AbpLanguages").OnColumn("IsDeleted");
        Alter.Column("CreationTime").OnTable("AbpLanguages").AsDateTime2().NotNullable();
        Alter.Column("LastModificationTime").OnTable("AbpLanguages").AsDateTime2().Nullable();
        Alter.Column("DeletionTime").OnTable("AbpLanguages").AsDateTime2().Nullable();
        #endregion
        #region AbpLanguageTexts

        Delete.Index("IX_TenantId_LanguageName_Source_Key").OnTable("AbpLanguageTexts");
        Alter.Column("LanguageName").OnTable("AbpLanguageTexts").AsString(ApplicationLanguage.MaxNameLength).NotNullable();
        Create.Index().OnTable("AbpLanguageTexts")
            .OnColumn("TenantId").Ascending()
            .OnColumn("Source").Ascending()
            .OnColumn("LanguageName").Ascending()
            .OnColumn("Key").Ascending()
            .WithOptions().NonClustered();

        Delete.DefaultConstraint().OnTable("AbpLanguageTexts").OnColumn("CreationTime");
        Alter.Column("CreationTime").OnTable("AbpLanguageTexts").AsDateTime2().NotNullable();
        Alter.Column("LastModificationTime").OnTable("AbpLanguageTexts").AsDateTime2().Nullable();
        #endregion
        #region AbpOrganizationUnits

        Delete.Index("IX_ParentId_Code").OnTable("AbpOrganizationUnits");
        Delete.Index("IX_TenantId_Code").OnTable("AbpOrganizationUnits");
        Alter.Column("Code").OnTable("AbpOrganizationUnits").AsString(OrganizationUnit.MaxCodeLength).NotNullable();
        Create.Index().OnTable("AbpOrganizationUnits")
            .OnColumn("TenantId").Ascending()
            .OnColumn("Code").Ascending()
            .WithOptions().NonClustered();

        Delete.DefaultConstraint().OnTable("AbpOrganizationUnits").OnColumn("CreationTime");
        Delete.DefaultConstraint().OnTable("AbpOrganizationUnits").OnColumn("IsDeleted");
        Alter.Column("CreationTime").OnTable("AbpOrganizationUnits").AsDateTime2().NotNullable();
        Alter.Column("LastModificationTime").OnTable("AbpOrganizationUnits").AsDateTime2().Nullable();
        Alter.Column("DeletionTime").OnTable("AbpOrganizationUnits").AsDateTime2().Nullable();
        Alter.Column("DisplayName").OnTable("AbpOrganizationUnits").AsString(OrganizationUnit.MaxDisplayNameLength).NotNullable();

        #endregion
        #region AbpPermissions
        Alter.Column("Discriminator").OnTable("AbpPermissions").AsString(int.MaxValue).NotNullable();
        Delete.DefaultConstraint().OnTable("AbpPermissions").OnColumn("CreationTime");
        Delete.DefaultConstraint().OnTable("AbpPermissions").OnColumn("IsGranted");
        Alter.Column("CreationTime").OnTable("AbpPermissions").AsDateTime2().NotNullable();
        #endregion
        #region AbpRoles
        Alter.Column("Name").OnTable("AbpRoles").AsString(AbpRoleBase.MaxNameLength).NotNullable();
        Alter.Column("DisplayName").OnTable("AbpRoles").AsString(AbpRoleBase.MaxDisplayNameLength).NotNullable();
        Delete.DefaultConstraint().OnTable("AbpRoles").OnColumn("CreationTime");
        Alter.Column("CreationTime").OnTable("AbpRoles").AsDateTime2().NotNullable();
        Alter.Column("LastModificationTime").OnTable("AbpRoles").AsDateTime2().Nullable();
        Alter.Column("DeletionTime").OnTable("AbpRoles").AsDateTime2().Nullable();
        #endregion
        #region AbpSettings
        Alter.Column("Name").OnTable("AbpSettings").AsString(Setting.MaxNameLength).NotNullable();
        Alter.Column("Value").OnTable("AbpSettings").AsString(int.MaxValue).Nullable();
        Delete.DefaultConstraint().OnTable("AbpSettings").OnColumn("CreationTime");
        Alter.Column("CreationTime").OnTable("AbpSettings").AsDateTime2().NotNullable();
        Alter.Column("LastModificationTime").OnTable("AbpSettings").AsDateTime2().Nullable();
        #endregion
        #region AbpTenants
        Delete.DefaultConstraint().OnTable("AbpTenants").OnColumn("CreationTime");
        Delete.DefaultConstraint().OnTable("AbpTenants").OnColumn("IsActive");
        Delete.DefaultConstraint().OnTable("AbpTenants").OnColumn("IsDeleted");
        Alter.Column("CreationTime").OnTable("AbpTenants").AsDateTime2().NotNullable();
        Alter.Column("LastModificationTime").OnTable("AbpTenants").AsDateTime2().Nullable();
        Alter.Column("DeletionTime").OnTable("AbpTenants").AsDateTime2().Nullable();
        #endregion
        #region AbpUserLoginAttempts
        Alter.Column("UserNameOrEmailAddress").OnTable("AbpUserLoginAttempts").AsString(UserLoginAttempt.MaxUserNameOrEmailAddressLength).Nullable();
        Alter.Column("BrowserInfo").OnTable("AbpUserLoginAttempts").AsString(UserLoginAttempt.MaxBrowserInfoLength).Nullable();
        Delete.DefaultConstraint().OnTable("AbpUserLoginAttempts").OnColumn("CreationTime");
        Alter.Column("CreationTime").OnTable("AbpUserLoginAttempts").AsDateTime2().NotNullable();
        #endregion
        #region AbpUserOrganizationUnits
        Delete.DefaultConstraint().OnTable("AbpUserOrganizationUnits").OnColumn("CreationTime");
        Alter.Column("CreationTime").OnTable("AbpUserOrganizationUnits").AsDateTime2().NotNullable();
        #endregion
        #region AbpUserRoles
        Delete.DefaultConstraint().OnTable("AbpUserRoles").OnColumn("CreationTime");
        Alter.Column("CreationTime").OnTable("AbpUserRoles").AsDateTime2().NotNullable();
        #endregion
        #region AbpUsers
        Delete.DefaultConstraint().OnTable("AbpUsers").OnColumn("CreationTime");
        Alter.Column("CreationTime").OnTable("AbpUsers").AsDateTime2().NotNullable();
        Alter.Column("LastModificationTime").OnTable("AbpUsers").AsDateTime2().Nullable();
        Alter.Column("DeletionTime").OnTable("AbpUsers").AsDateTime2().Nullable();

        Delete.Column("LastLoginTime").FromTable("AbpUsers");

        #endregion
    }

    public override void Down()
    {
        #region AbpAuditLogs
        Delete.Index("IX_TenantId_ExecutionTime").OnTable("AbpAuditLogs");
        Delete.Index("IX_UserId_ExecutionTime").OnTable("AbpAuditLogs");

        Alter.Column("ExecutionTime").OnTable("AbpAuditLogs").AsDateTime().NotNullable();

        Create.Index("IX_TenantId_ExecutionTime").OnTable("AbpAuditLogs")
            .OnColumn("TenantId").Ascending()
            .OnColumn("ExecutionTime").Ascending()
            .WithOptions().NonClustered();
        Create.Index("IX_UserId_ExecutionTime").OnTable("AbpAuditLogs")
            .OnColumn("UserId").Ascending()
            .OnColumn("ExecutionTime").Ascending()
            .WithOptions().NonClustered();
        Alter.Column("BrowserInfo").OnTable("AbpAuditLogs").AsString(256).Nullable();
        #endregion
        #region AbpBackgroundJobs
        Delete.DefaultConstraint().OnTable("AbpBackgroundJobs").OnColumn("TryCount");
        Delete.DefaultConstraint().OnTable("AbpBackgroundJobs").OnColumn("IsAbandoned");
        Delete.DefaultConstraint().OnTable("AbpBackgroundJobs").OnColumn("Priority");
        Alter.Table("AbpBackgroundJobs").AlterColumn("TryCount").AsInt16().NotNullable().WithDefaultValue(0);
        Alter.Table("AbpBackgroundJobs").AlterColumn("IsAbandoned").AsBoolean().Nullable().WithDefaultValue(false);
        Alter.Table("AbpBackgroundJobs").AlterColumn("Priority").AsByte().NotNullable().WithDefaultValue(15);

        Delete.Index("IX_AbpBackgroundJobs_IsAbandoned_NextTryTime").OnTable("AbpBackgroundJobs");

        Alter.Column("NextTryTime").OnTable("AbpBackgroundJobs").AsDateTime().NotNullable();

        Create.Index("IX_IsAbandoned_NextTryTime").OnTable("AbpBackgroundJobs")
            .OnColumn("IsAbandoned").Ascending()
            .OnColumn("NextTryTime").Ascending()
            .WithOptions().NonClustered();

        Alter.Column("CreationTime").OnTable("AbpBackgroundJobs").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        Alter.Column("LastTryTime").OnTable("AbpBackgroundJobs").AsDateTime().Nullable();
        #endregion
        #region AbpEditions
        Alter.Column("CreationTime").OnTable("AbpEditions").AsDateTime().WithDefault(SystemMethods.CurrentDateTime).NotNullable();
        Alter.Column("LastModificationTime").OnTable("AbpEditions").AsDateTime().Nullable();
        Alter.Column("DeletionTime").OnTable("AbpEditions").AsDateTime().Nullable();
        Alter.Column("IsDeleted").OnTable("AbpEditions").AsBoolean().NotNullable().WithDefaultValue(false);
        #endregion
        #region AbpFeatures
        Alter.Column("CreationTime").OnTable("AbpFeatures").AsDateTime().WithDefault(SystemMethods.CurrentDateTime).NotNullable();

        Alter.Column("Discriminator").OnTable("AbpFeatures").AsString(128).NotNullable();

        Create.Index("IX_Discriminator_EditionId_Name").OnTable("AbpFeatures")
            .OnColumn("Discriminator").Ascending()
            .OnColumn("EditionId").Ascending()
            .OnColumn("Name").Ascending()
            .WithOptions().NonClustered();

        Create.Index("IX_Discriminator_TenantId_Name").OnTable("AbpFeatures")
            .OnColumn("Discriminator").Ascending()
            .OnColumn("TenantId").Ascending()
            .OnColumn("Name").Ascending()
            .WithOptions().NonClustered();
        #endregion
        #region AbpLanguages

        Delete.Index("IX_AbpLanguages_TenantId_Name").OnTable("AbpLanguages");
        Alter.Column("Name").OnTable("AbpLanguages").AsString(10).NotNullable();
        Create.Index("IX_TenantId_Name").OnTable("AbpLanguages")
            .OnColumn("TenantId").Ascending()
            .OnColumn("Name").Ascending()
            .WithOptions().NonClustered();

        Alter.Column("CreationTime").OnTable("AbpLanguages").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        Alter.Column("LastModificationTime").OnTable("AbpLanguages").AsDateTime().Nullable();
        Alter.Column("DeletionTime").OnTable("AbpLanguages").AsDateTime().Nullable();
        Alter.Column("IsDeleted").OnTable("AbpLanguages").AsBoolean().NotNullable().WithDefaultValue(false);
        #endregion
        #region AbpLanguageTexts

        Delete.Index("IX_AbpLanguageTexts_TenantId_Source_LanguageName_Key").OnTable("AbpLanguageTexts");
        Alter.Column("LanguageName").OnTable("AbpLanguageTexts").AsString(10).NotNullable();
        Create.Index("IX_TenantId_LanguageName_Source_Key").OnTable("AbpLanguageTexts")
            .OnColumn("TenantId").Ascending()
            .OnColumn("LanguageName").Ascending()
            .OnColumn("Source").Ascending()
            .OnColumn("Key").Ascending()
            .WithOptions().NonClustered();

        Alter.Column("CreationTime").OnTable("AbpLanguageTexts").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        Alter.Column("LastModificationTime").OnTable("AbpLanguageTexts").AsDateTime().Nullable();
        #endregion
        #region AbpOrganizationUnits
        Delete.Index("IX_AbpOrganizationUnits_TenantId_Code").OnTable("AbpOrganizationUnits");
        Alter.Column("Code").OnTable("AbpOrganizationUnits").AsString(128).NotNullable();
        Create.Index("IX_ParentId_Code").OnTable("AbpOrganizationUnits")
            .OnColumn("ParentId").Ascending()
            .OnColumn("Code").Ascending()
            .WithOptions().NonClustered();
        Create.Index("IX_TenantId_Code").OnTable("AbpOrganizationUnits")
            .OnColumn("TenantId").Ascending()
            .OnColumn("Code").Ascending()
            .WithOptions().NonClustered();

        Alter.Column("CreationTime").OnTable("AbpOrganizationUnits").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        Alter.Column("IsDeleted").OnTable("AbpOrganizationUnits").AsBoolean().NotNullable().WithDefaultValue(false);
        Alter.Column("LastModificationTime").OnTable("AbpOrganizationUnits").AsDateTime().Nullable();
        Alter.Column("DeletionTime").OnTable("AbpOrganizationUnits").AsDateTime().Nullable();
        Alter.Column("DisplayName").OnTable("AbpOrganizationUnits").AsString(128).NotNullable();
        #endregion
        #region AbpPermissions
        Alter.Column("Discriminator").OnTable("AbpPermissions").AsString(128).NotNullable();
        Alter.Column("CreationTime").OnTable("AbpPermissions").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        Alter.Column("IsGranted").OnTable("AbpPermissions").AsBoolean().NotNullable().WithDefaultValue(false);
        #endregion
        #region AbpRoles
        Alter.Column("Name").OnTable("AbpRoles").AsString(32).NotNullable();
        Alter.Column("DisplayName").OnTable("AbpRoles").AsString(64).NotNullable();
        Alter.Column("CreationTime").OnTable("AbpRoles").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        Alter.Column("LastModificationTime").OnTable("AbpRoles").AsDateTime().Nullable();
        Alter.Column("DeletionTime").OnTable("AbpRoles").AsDateTime().Nullable();
        #endregion
        #region AbpSettings
        Alter.Column("Name").OnTable("AbpSettings").AsString(256).NotNullable();
        Alter.Column("Value").OnTable("AbpSettings").AsString(2000).NotNullable();
        Delete.DefaultConstraint().OnTable("AbpSettings").OnColumn("CreationTime");
        Alter.Column("CreationTime").OnTable("AbpSettings").AsDateTime().NotNullable();
        Alter.Column("LastModificationTime").OnTable("AbpSettings").AsDateTime().Nullable();
        #endregion
        #region AbpTenants
        Alter.Column("CreationTime").OnTable("AbpTenants").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        Alter.Column("LastModificationTime").OnTable("AbpTenants").AsDateTime().Nullable();
        Alter.Column("DeletionTime").OnTable("AbpTenants").AsDateTime().Nullable();

        Alter.Column("IsActive").OnTable("AbpTenants").AsBoolean().NotNullable().WithDefaultValue(true);
        Alter.Column("IsDeleted").OnTable("AbpTenants").AsBoolean().NotNullable().WithDefaultValue(false);
        #endregion
        #region AbpUserLoginAttempts
        Alter.Column("UserNameOrEmailAddress").OnTable("AbpUserLoginAttempts").AsString(255).Nullable();
        Alter.Column("BrowserInfo").OnTable("AbpUserLoginAttempts").AsString(256).Nullable();
        Alter.Column("CreationTime").OnTable("AbpUserLoginAttempts").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        #endregion
        #region AbpUserOrganizationUnits
        Alter.Column("CreationTime").OnTable("AbpUserOrganizationUnits").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        #endregion
        #region AbpUserRoles
        Alter.Column("CreationTime").OnTable("AbpUserRoles").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        #endregion
        #region AbpUsers
        Alter.Column("CreationTime").OnTable("AbpUsers").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        Alter.Column("LastModificationTime").OnTable("AbpUsers").AsDateTime().Nullable();
        Alter.Column("DeletionTime").OnTable("AbpUsers").AsDateTime().Nullable();

        Create.Column("LastLoginTime").OnTable("AbpUsers").AsDateTime2().Nullable();

        #endregion
    }
}