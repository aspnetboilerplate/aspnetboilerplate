using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.DynamicEntityProperties;
using Abp.EntityHistory;
using Abp.FluentMigrator.Extensions;
using Abp.Notifications;
using FluentMigrator;
using FluentMigrator.SqlServer;
using System.Data;

namespace Abp.Zero.FluentMigrator.Migrations;

[Migration(2022_10_24_03)]
public class _2022_10_24_03_Add_Missing_Tables : AutoReversingMigration
{
    public override void Up()
    {
        #region AbpDynamicProperties
        Create.Table("AbpDynamicProperties")
            .WithIdAsInt32()
            .WithColumn("PropertyName").AsString(DynamicProperty.MaxPropertyName).Nullable()
            .WithColumn("InputType").AsString(int.MaxValue).Nullable()
            .WithColumn("Permission").AsString(int.MaxValue).Nullable()
            .WithTenantIdAsNullable()
            .WithColumn("DisplayName").AsString(int.MaxValue).Nullable();

        Create.Index().OnTable("AbpDynamicProperties")
            .OnColumn("PropertyName").Unique()
            .OnColumn("TenantId").Unique()
            .WithOptions().Filter("[PropertyName] IS NOT NULL AND [TenantId] IS NOT NULL");
        #endregion
        #region AbpDynamicPropertyValues
        Create.Table("AbpDynamicPropertyValues")
            .WithIdAsInt64()
            .WithColumn("Value").AsString(int.MaxValue).NotNullable()
            .WithTenantIdAsNullable()
            .WithColumn("DynamicPropertyId").AsInt32().NotNullable();

        Create.ForeignKey()
            .FromTable("AbpDynamicPropertyValues").ForeignColumn("DynamicPropertyId")
            .ToTable("AbpDynamicProperties").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);

        Create.Index().OnTable("AbpDynamicPropertyValues")
            .OnColumn("DynamicPropertyId").Ascending()
            .WithOptions().NonClustered();
        #endregion
        #region AbpDynamicEntityProperties
        Create.Table("AbpDynamicEntityProperties")
            .WithIdAsInt32()
            .WithColumn("EntityFullName").AsString(DynamicEntityProperty.MaxEntityFullName).Nullable()
            .WithColumn("DynamicPropertyId").AsInt32().NotNullable()
            .WithTenantIdAsNullable();

        Create.Index().OnTable("AbpDynamicEntityProperties")
            .OnColumn("DynamicPropertyId").Ascending()
            .WithOptions().NonClustered();

        Create.ForeignKey()
            .FromTable("AbpDynamicEntityProperties").ForeignColumn("DynamicPropertyId")
            .ToTable("AbpDynamicProperties").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);

        Create.Index().OnTable("AbpDynamicEntityProperties")
            .OnColumn("EntityFullName").Unique()
            .OnColumn("DynamicPropertyId").Unique()
            .OnColumn("TenantId").Unique()
            .WithOptions().Filter("[EntityFullName] IS NOT NULL AND [TenantId] IS NOT NULL");
        #endregion
        #region AbpDynamicEntityPropertyValues
        Create.Table("AbpDynamicEntityPropertyValues")
            .WithIdAsInt64()
            .WithColumn("Value").AsString(int.MaxValue).NotNullable()
            .WithColumn("EntityId").AsString(int.MaxValue).Nullable()
            .WithColumn("DynamicEntityPropertyId").AsInt32().NotNullable()
            .WithTenantIdAsNullable();

        Create.Index().OnTable("AbpDynamicEntityPropertyValues")
            .OnColumn("DynamicEntityPropertyId").Ascending()
            .WithOptions().NonClustered();

        Create.ForeignKey()
            .FromTable("AbpDynamicEntityPropertyValues").ForeignColumn("DynamicEntityPropertyId")
            .ToTable("AbpDynamicEntityProperties").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);
        #endregion
        #region AbpEntityChangeSets

        Create.Table("AbpEntityChangeSets")
            .WithIdAsInt64()
            .WithColumn("BrowserInfo").AsString(EntityChangeSet.MaxBrowserInfoLength).Nullable()
            .WithColumn("ClientIpAddress").AsString(EntityChangeSet.MaxClientIpAddressLength).Nullable()
            .WithColumn("ClientName").AsString(EntityChangeSet.MaxClientNameLength).Nullable()
            .WithColumn("CreationTime").AsDateTime2().NotNullable()
            .WithColumn("ExtensionData").AsString(int.MaxValue).Nullable()
            .WithColumn("ImpersonatorTenantId").AsInt32().Nullable()
            .WithColumn("ImpersonatorUserId").AsInt64().Nullable()
            .WithColumn("Reason").AsString(EntityChangeSet.MaxReasonLength).Nullable()
            .WithColumn("TenantId").AsInt32().Nullable()
            .WithColumn("UserId").AsInt64().Nullable();

        Create.Index().OnTable("AbpEntityChangeSets")
            .OnColumn("TenantId").Ascending()
            .OnColumn("CreationTime").Ascending()
            .WithOptions().NonClustered();

        Create.Index().OnTable("AbpEntityChangeSets")
            .OnColumn("TenantId").Ascending()
            .OnColumn("Reason").Ascending()
            .WithOptions().NonClustered();

        Create.Index().OnTable("AbpEntityChangeSets")
            .OnColumn("TenantId").Ascending()
            .OnColumn("UserId").Ascending()
            .WithOptions().NonClustered();

        #endregion
        #region AbpEntityChanges

        Create.Table("AbpEntityChanges")
            .WithIdAsInt64()
            .WithColumn("ChangeTime").AsDateTime2().NotNullable()
            .WithColumn("ChangeType").AsByte().NotNullable()
            .WithColumn("EntityChangeSetId").AsInt64().NotNullable().ForeignKey("AbpEntityChangeSets", "Id")
            .OnDelete(Rule.Cascade)
            .WithColumn("EntityId").AsString(EntityChange.MaxEntityIdLength).Nullable()
            .WithColumn("EntityTypeFullName").AsString(EntityChange.MaxEntityTypeFullNameLength).Nullable()
            .WithColumn("TenantId").AsInt32().Nullable();

        Create.Index().OnTable("AbpEntityChanges")
            .OnColumn("EntityChangeSetId").Ascending()
            .WithOptions().NonClustered();

        Create.Index().OnTable("AbpEntityChanges")
            .OnColumn("EntityTypeFullName").Ascending()
            .OnColumn("EntityId").Ascending()
            .WithOptions().NonClustered();
        #endregion
        #region AbpEntityPropertyChanges

        Create.Table("AbpEntityPropertyChanges")
            .WithIdAsInt64()
            .WithColumn("EntityChangeId").AsInt64().NotNullable().ForeignKey("AbpEntityChanges", "Id")
            .OnDelete(Rule.Cascade)
            .WithColumn("NewValue").AsString(EntityPropertyChange.MaxValueLength).Nullable()
            .WithColumn("OriginalValue").AsString(EntityPropertyChange.MaxValueLength).Nullable()
            .WithColumn("PropertyName").AsString(EntityPropertyChange.MaxPropertyNameLength).Nullable()
            .WithColumn("PropertyTypeFullName").AsString(EntityPropertyChange.MaxPropertyTypeFullNameLength).Nullable()
            .WithTenantIdAsNullable()
            .WithColumn("NewValueHash").AsString(int.MaxValue).Nullable()
            .WithColumn("OriginalValueHash").AsString(int.MaxValue).Nullable();

        Create.Index().OnTable("AbpEntityPropertyChanges")
            .OnColumn("EntityChangeId").Ascending()
            .WithOptions().NonClustered();
        #endregion
        #region AbpNotifications

        Create.Table("AbpNotifications")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithCreationAuditColumns()
            .WithColumn("Data").AsString(int.MaxValue).Nullable()
            .WithColumn("DataTypeName").AsString(NotificationInfo.MaxDataTypeNameLength).Nullable()
            .WithColumn("EntityId").AsString(NotificationInfo.MaxEntityIdLength).Nullable()
            .WithColumn("EntityTypeAssemblyQualifiedName").AsString(NotificationInfo.MaxEntityTypeAssemblyQualifiedNameLength).Nullable()
            .WithColumn("EntityTypeName").AsString(NotificationInfo.MaxEntityTypeNameLength).Nullable()
            .WithColumn("ExcludedUserIds").AsString(int.MaxValue).Nullable()
            .WithColumn("NotificationName").AsString(NotificationInfo.MaxNotificationNameLength).NotNullable()
            .WithColumn("Severity").AsByte().NotNullable()
            .WithColumn("TenantIds").AsString(int.MaxValue).Nullable()
            .WithColumn("UserIds").AsString(int.MaxValue).Nullable()
            .WithColumn("TargetNotifiers").AsString(int.MaxValue).Nullable();
        #endregion
        #region AbpNotificationSubscriptions

        Create.Table("AbpNotificationSubscriptions")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithCreationAuditColumns()
            .WithColumn("EntityId").AsString(NotificationInfo.MaxEntityIdLength).Nullable()
            .WithColumn("EntityTypeAssemblyQualifiedName").AsString(NotificationInfo.MaxEntityTypeAssemblyQualifiedNameLength).Nullable()
            .WithColumn("EntityTypeName").AsString(NotificationInfo.MaxEntityTypeNameLength).Nullable()
            .WithColumn("NotificationName").AsString(NotificationInfo.MaxNotificationNameLength).Nullable()
            .WithTenantIdAsNullable()
            .WithUserId();

        Create.Index().OnTable("AbpNotificationSubscriptions")
            .OnColumn("NotificationName").Ascending()
            .OnColumn("EntityTypeName").Ascending()
            .OnColumn("EntityId").Ascending()
            .OnColumn("UserId").Ascending()
            .WithOptions().NonClustered();

        Create.Index().OnTable("AbpNotificationSubscriptions")
            .OnColumn("TenantId").Ascending()
            .OnColumn("NotificationName").Ascending()
            .OnColumn("EntityTypeName").Ascending()
            .OnColumn("EntityId").Ascending()
            .OnColumn("UserId").Ascending()
            .WithOptions().NonClustered();
        #endregion
        #region AbpTenantNotifications

        Create.Table("AbpTenantNotifications")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithCreationAuditColumns()
            .WithColumn("Data").AsString(int.MaxValue).Nullable()
            .WithColumn("DataTypeName").AsString(NotificationInfo.MaxDataTypeNameLength).Nullable()
            .WithColumn("EntityId").AsString(NotificationInfo.MaxEntityIdLength).Nullable()
            .WithColumn("EntityTypeAssemblyQualifiedName").AsString(NotificationInfo.MaxEntityTypeAssemblyQualifiedNameLength).Nullable()
            .WithColumn("EntityTypeName").AsString(NotificationInfo.MaxEntityTypeNameLength).Nullable()
            .WithColumn("NotificationName").AsString(NotificationInfo.MaxNotificationNameLength).NotNullable()
            .WithColumn("Severity").AsByte().NotNullable()
            .WithTenantIdAsNullable();

        Create.Index().OnTable("AbpTenantNotifications")
            .OnColumn("TenantId").Ascending()
            .WithOptions().NonClustered();
        #endregion
        #region AbpUserNotifications

        Create.Table("AbpUserNotifications")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithCreationTimeColumn()
            .WithColumn("State").AsInt32().NotNullable()
            .WithTenantIdAsNullable()
            .WithColumn("TenantNotificationId").AsGuid().NotNullable()
            .WithUserId()
            .WithColumn("TargetNotifiers").AsString(NotificationInfo.MaxTargetNotifiersLength).Nullable();

        Create.Index().OnTable("AbpUserNotifications")
            .OnColumn("UserId").Ascending()
            .OnColumn("State").Ascending()
            .OnColumn("CreationTime").Ascending()
            .WithOptions().NonClustered();
        #endregion
        #region AbpRoleClaims

        Create.Table("AbpRoleClaims")
            .WithIdAsInt64()
            .WithColumn("ClaimType").AsString(RoleClaim.MaxClaimTypeLength).Nullable()
            .WithColumn("ClaimValue").AsString(int.MaxValue).Nullable()
            .WithCreationAuditColumns()
            .WithColumn("RoleId").AsInt32().NotNullable().ForeignKey("AbpRoles", "Id").OnDelete(Rule.Cascade)
            .WithTenantIdAsNullable();

        Create.Index().OnTable("AbpRoleClaims")
            .OnColumn("RoleId").Ascending()
            .WithOptions().NonClustered();

        Create.Index().OnTable("AbpRoleClaims")
            .OnColumn("TenantId").Ascending()
            .OnColumn("ClaimType").Ascending()
            .WithOptions().NonClustered();
        #endregion
        #region AbpUserClaims

        Create.Table("AbpUserClaims")
            .WithIdAsInt64()
            .WithColumn("ClaimType").AsString(UserClaim.MaxClaimTypeLength).Nullable()
            .WithColumn("ClaimValue").AsString(int.MaxValue).Nullable()
            .WithCreationAuditColumns()
            .WithTenantIdAsNullable()
            .WithUserId();

        Create.Index().OnTable("AbpUserClaims")
            .OnColumn("TenantId").Ascending()
            .OnColumn("ClaimType").Ascending()
            .WithOptions().NonClustered();

        Create.Index().OnTable("AbpUserClaims")
            .OnColumn("UserId").Ascending()
            .WithOptions().NonClustered();
        #endregion
        #region AbpUserAccounts

        Create.Table("AbpUserAccounts")
            .WithIdAsInt64()
            .WithCreationAuditColumns()
            .WithDeletionAuditColumns()
            .WithColumn("EmailAddress").AsString(UserAccount.MaxEmailAddressLength).Nullable()
            .WithModificationAuditColumns()
            .WithTenantIdAsNullable()
            .WithUserId()
            .WithColumn("UserLinkId").AsInt64().Nullable()
            .WithColumn("UserName").AsString(UserAccount.MaxUserNameLength).Nullable();

        Create.Index().OnTable("AbpUserAccounts")
            .OnColumn("EmailAddress").Ascending()
            .WithOptions().NonClustered();

        Create.Index().OnTable("AbpUserAccounts")
            .OnColumn("TenantId").Ascending()
            .OnColumn("EmailAddress").Ascending()
            .WithOptions().NonClustered();

        Create.Index().OnTable("AbpUserAccounts")
            .OnColumn("TenantId").Ascending()
            .OnColumn("UserId").Ascending()
            .WithOptions().NonClustered();

        Create.Index().OnTable("AbpUserAccounts")
            .OnColumn("TenantId").Ascending()
            .OnColumn("UserName").Ascending()
            .WithOptions().NonClustered();

        Create.Index().OnTable("AbpUserAccounts")
            .OnColumn("UserName").Ascending()
            .WithOptions().NonClustered();
        #endregion
        #region AbpUserTokens

        Create.Table("AbpUserTokens")
            .WithIdAsInt64()
            .WithColumn("LoginProvider").AsString(UserToken.MaxLoginProviderLength).Nullable()
            .WithColumn("Name").AsString(UserToken.MaxNameLength).Nullable()
            .WithTenantIdAsNullable()
            .WithUserId()
            .WithColumn("Value").AsString(UserToken.MaxValueLength).Nullable()
            .WithColumn("ExpireDate").AsDateTime2().Nullable();

        Create.Index().OnTable("AbpUserTokens")
            .OnColumn("TenantId").Ascending()
            .OnColumn("UserId").Ascending()
            .WithOptions().NonClustered();

        Create.Index().OnTable("AbpUserTokens")
            .OnColumn("UserId").Ascending()
            .WithOptions().NonClustered();
        #endregion
        #region AbpOrganizationUnitRoles

        Create.Table("AbpOrganizationUnitRoles")
            .WithIdAsInt64()
            .WithCreationAuditColumns()
            .WithTenantIdAsNullable()
            .WithColumn("RoleId").AsInt32().NotNullable().ForeignKey("AbpRoles", "Id").OnDelete(Rule.Cascade)
            .WithColumn("OrganizationUnitId").AsInt64().ForeignKey("AbpOrganizationUnits", "Id")
            .OnDelete(Rule.Cascade)
            .WithIsDeletedColumn();

        Create.Index().OnTable("AbpOrganizationUnitRoles")
            .OnColumn("TenantId").Ascending()
            .OnColumn("OrganizationUnitId").Ascending()
            .WithOptions().NonClustered();

        Create.Index().OnTable("AbpOrganizationUnitRoles")
            .OnColumn("TenantId").Ascending()
            .OnColumn("RoleId").Ascending()
            .WithOptions().NonClustered();
        #endregion
        #region AbpWebhookEvents

        Create.Table("AbpWebhookEvents")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("WebhookName").AsString(int.MaxValue).NotNullable()
            .WithColumn("Data").AsString(int.MaxValue).Nullable()
            .WithCreationTimeColumn()
            .WithTenantIdAsNullable()
            .WithIsDeletedColumn()
            .WithDeletionTimeColumn();
        #endregion
        #region AbpWebhookSubscriptions

        Create.Table("AbpWebhookSubscriptions")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithCreationAuditColumns()
            .WithTenantIdAsNullable()
            .WithColumn("WebhookUri").AsString(int.MaxValue).NotNullable()
            .WithColumn("Secret").AsString(int.MaxValue).NotNullable()
            .WithIsActiveColumn()
            .WithColumn("Webhooks").AsString(int.MaxValue).Nullable()
            .WithColumn("Headers").AsString(int.MaxValue).Nullable();
        #endregion
        #region AbpWebhookSendAttempts

        Create.Table("AbpWebhookSendAttempts")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("WebhookEventId").AsGuid().NotNullable().ForeignKey("AbpWebhookEvents", "Id")
            .WithColumn("WebhookSubscriptionId").AsGuid().NotNullable().ForeignKey("AbpWebhookSubscriptions", "Id")
            .WithColumn("Response").AsString(int.MaxValue).Nullable()
            .WithColumn("ResponseStatusCode").AsInt32().Nullable()
            .WithCreationTimeColumn()
            .WithLastModificationTimeColumn()
            .WithTenantIdAsNullable();

        Create.Index().OnTable("AbpWebhookSendAttempts")
            .OnColumn("WebhookEventId").Ascending()
            .WithOptions().NonClustered();
        #endregion
    }
}