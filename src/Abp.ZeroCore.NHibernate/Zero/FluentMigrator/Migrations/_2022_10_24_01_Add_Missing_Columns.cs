using Abp.Auditing;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations;

[Migration(2022_10_24_01)]
public class _2022_10_24_01_Add_Missing_Columns : AutoReversingMigration
{
    public override void Up()
    {
        Create.Column("ReturnValue").OnTable("AbpAuditLogs").AsString(int.MaxValue).Nullable();
        Create.Column("ExceptionMessage").OnTable("AbpAuditLogs").AsString(AuditLog.MaxExceptionMessageLength).Nullable();

        Create.Column("IsDisabled").OnTable("AbpLanguages").AsBoolean().NotNullable().WithDefaultValue(false);

        Create.Column("TenantId").OnTable("AbpPermissions").AsInt32().Nullable().ForeignKey("AbpTenants", "Id");

        Create.Column("ConcurrencyStamp").OnTable("AbpRoles").AsString(128).Nullable();
        Create.Column("NormalizedName").OnTable("AbpRoles").AsString(AbpRoleBase.MaxNameLength).NotNullable().SetExistingRowsTo(RawSql.Insert("UPPER(Name)"));
        Create.Column("Description").OnTable("AbpRoles").AsString(int.MaxValue).Nullable();

        Alter.Column("Value").OnTable("AbpSettings").AsString(int.MaxValue).Nullable();

        Create.Column("ConnectionString").OnTable("AbpTenants").AsString(AbpTenantBase.MaxConnectionStringLength).Nullable();

        Create.Column("Id").OnTable("AbpUserLoginAttempts").AsInt64().PrimaryKey().Identity();

        Create.Column("TenantId").OnTable("AbpUserLogins").AsInt32().Nullable().ForeignKey("AbpTenants", "Id");

        Create.Column("IsDeleted").OnTable("AbpUserOrganizationUnits").AsBoolean().NotNullable().WithDefaultValue(false);

        Create.Column("TenantId").OnTable("AbpUserRoles").AsInt32().Nullable().ForeignKey("AbpTenants", "Id");

        Create.Column("AccessFailedCount").OnTable("AbpUsers").AsInt32().NotNullable().SetExistingRowsTo(0);
        Create.Column("ConcurrencyStamp").OnTable("AbpUsers").AsString(128).Nullable();
        Alter.Column("EmailConfirmationCode").OnTable("AbpUsers").AsString(AbpUserBase.MaxEmailConfirmationCodeLength).Nullable();
        Create.Column("IsLockoutEnabled").OnTable("AbpUsers").AsBoolean().NotNullable().SetExistingRowsTo(false);
        Create.Column("IsPhoneNumberConfirmed").OnTable("AbpUsers").AsBoolean().NotNullable().SetExistingRowsTo(false);
        Create.Column("IsTwoFactorEnabled").OnTable("AbpUsers").AsBoolean().NotNullable().SetExistingRowsTo(false);
        Create.Column("LockoutEndDateUtc").OnTable("AbpUsers").AsDateTime2().Nullable();
        Alter.Column("Name").OnTable("AbpUsers").AsString(AbpUserBase.MaxNameLength).NotNullable();
        Create.Column("NormalizedEmailAddress").OnTable("AbpUsers").AsString(AbpUserBase.MaxEmailAddressLength).NotNullable().SetExistingRowsTo(RawSql.Insert("UPPER(EmailAddress)"));
        Create.Column("NormalizedUserName").OnTable("AbpUsers").AsString(AbpUserBase.MaxUserNameLength).NotNullable().SetExistingRowsTo(RawSql.Insert("UPPER(UserName)"));
        Create.Column("PhoneNumber").OnTable("AbpUsers").AsString(AbpUserBase.MaxPhoneNumberLength).Nullable();
        Create.Column("SecurityStamp").OnTable("AbpUsers").AsString(AbpUserBase.MaxSecurityStampLength).Nullable();
        Alter.Column("Surname").OnTable("AbpUsers").AsString(AbpUserBase.MaxSurnameLength).NotNullable();
        Alter.Column("UserName").OnTable("AbpUsers").AsString(AbpUserBase.MaxUserNameLength).NotNullable();
    }
}