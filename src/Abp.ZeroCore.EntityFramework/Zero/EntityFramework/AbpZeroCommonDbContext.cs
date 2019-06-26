using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.EntityFramework;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Abp.EntityFramework.Extensions;

namespace Abp.Zero.EntityFramework
{
    public abstract class AbpZeroCommonDbContext<TRole, TUser, TSelf> : AbpDbContext
        where TRole : AbpRole<TUser>
        where TUser : AbpUser<TUser>
        where TSelf : AbpZeroCommonDbContext<TRole, TUser, TSelf>
    {
        /// <summary>
        /// Roles.
        /// </summary>
        public virtual DbSet<TRole> Roles { get; set; }

        /// <summary>
        /// Users.
        /// </summary>
        public virtual DbSet<TUser> Users { get; set; }

        /// <summary>
        /// User logins.
        /// </summary>
        public virtual DbSet<UserLogin> UserLogins { get; set; }

        /// <summary>
        /// User login attempts.
        /// </summary>
        public virtual DbSet<UserLoginAttempt> UserLoginAttempts { get; set; }

        /// <summary>
        /// User roles.
        /// </summary>
        public virtual DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// User claims.
        /// </summary>
        public virtual DbSet<UserClaim> UserClaims { get; set; }

        /// <summary>
        /// User tokens.
        /// </summary>
        public virtual DbSet<UserToken> UserTokens { get; set; }

        /// <summary>
        /// Role claims.
        /// </summary>
        public virtual DbSet<RoleClaim> RoleClaims { get; set; }

        /// <summary>
        /// Permissions.
        /// </summary>
        public virtual DbSet<PermissionSetting> Permissions { get; set; }

        /// <summary>
        /// Role permissions.
        /// </summary>
        public virtual DbSet<RolePermissionSetting> RolePermissions { get; set; }

        /// <summary>
        /// User permissions.
        /// </summary>
        public virtual DbSet<UserPermissionSetting> UserPermissions { get; set; }

        /// <summary>
        /// Settings.
        /// </summary>
        public virtual DbSet<Setting> Settings { get; set; }

        /// <summary>
        /// Audit logs.
        /// </summary>
        public virtual DbSet<AuditLog> AuditLogs { get; set; }

        /// <summary>
        /// Languages.
        /// </summary>
        public virtual DbSet<ApplicationLanguage> Languages { get; set; }

        /// <summary>
        /// LanguageTexts.
        /// </summary>
        public virtual DbSet<ApplicationLanguageText> LanguageTexts { get; set; }

        /// <summary>
        /// OrganizationUnits.
        /// </summary>
        public virtual DbSet<OrganizationUnit> OrganizationUnits { get; set; }

        /// <summary>
        /// UserOrganizationUnits.
        /// </summary>
        public virtual DbSet<UserOrganizationUnit> UserOrganizationUnits { get; set; }

        /// <summary>
        /// OrganizationUnitRoles.
        /// </summary>
        public virtual DbSet<OrganizationUnitRole> OrganizationUnitRoles { get; set; }

        /// <summary>
        /// Tenant notifications.
        /// </summary>
        public virtual DbSet<TenantNotificationInfo> TenantNotifications { get; set; }

        /// <summary>
        /// User notifications.
        /// </summary>
        public virtual DbSet<UserNotificationInfo> UserNotifications { get; set; }

        /// <summary>
        /// Notification subscriptions.
        /// </summary>
        public virtual DbSet<NotificationSubscriptionInfo> NotificationSubscriptions { get; set; }

        /// <summary>
        /// Default constructor.
        /// Do not directly instantiate this class. Instead, use dependency injection!
        /// </summary>
        protected AbpZeroCommonDbContext()
        {

        }

        /// <summary>
        /// Constructor with connection string parameter.
        /// </summary>
        /// <param name="nameOrConnectionString">Connection string or a name in connection strings in configuration file</param>
        protected AbpZeroCommonDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        protected AbpZeroCommonDbContext(DbCompiledModel model)
            : base(model)
        {

        }

        /// <summary>
        /// This constructor can be used for unit tests.
        /// </summary>
        protected AbpZeroCommonDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {

        }

        protected AbpZeroCommonDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {

        }

        protected AbpZeroCommonDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpZeroCommonDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region TUser.Set_ConcurrencyStamp

            modelBuilder.Entity<TUser>()
                .Property(e => e.ConcurrencyStamp)
                .IsConcurrencyToken();

            #endregion

            #region TUser.Set_ForeignKeys

            modelBuilder.Entity<TUser>()
                .HasOptional(p => p.DeleterUser)
                .WithMany()
                .HasForeignKey(p => p.DeleterUserId);

            modelBuilder.Entity<TUser>()
                .HasOptional(p => p.CreatorUser)
                .WithMany()
                .HasForeignKey(p => p.CreatorUserId);

            modelBuilder.Entity<TUser>()
                .HasOptional(p => p.LastModifierUser)
                .WithMany()
                .HasForeignKey(p => p.LastModifierUserId);

            #endregion

            #region TRole.Set_ConcurrencyStamp

            modelBuilder.Entity<TRole>()
                .Property(e => e.ConcurrencyStamp)
                .IsConcurrencyToken();

            #endregion

            #region AuditLog.IX_TenantId_UserId

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_UserId", 1);

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.UserId)
                .CreateIndex("IX_TenantId_UserId", 2);

            #endregion

            #region AuditLog.IX_TenantId_ExecutionTime

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_ExecutionTime", 1);

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.ExecutionTime)
                .CreateIndex("IX_TenantId_ExecutionTime", 2);

            #endregion

            #region AuditLog.IX_TenantId_ExecutionDuration

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_ExecutionDuration", 1);

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.ExecutionDuration)
                .CreateIndex("IX_TenantId_ExecutionDuration", 2);

            #endregion

            #region ApplicationLanguage.IX_TenantId_Name

            modelBuilder.Entity<ApplicationLanguage>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_Name", 1);

            modelBuilder.Entity<ApplicationLanguage>()
                .Property(e => e.Name)
                .CreateIndex("IX_TenantId_Name", 2);

            #endregion

            #region ApplicationLanguageText.IX_TenantId_Source_LanguageName_Key

            modelBuilder.Entity<ApplicationLanguageText>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_Source_LanguageName_Key", 1);

            modelBuilder.Entity<ApplicationLanguageText>()
                .Property(e => e.Source)
                .CreateIndex("IX_TenantId_Source_LanguageName_Key", 2);

            modelBuilder.Entity<ApplicationLanguageText>()
                .Property(e => e.LanguageName)
                .CreateIndex("IX_TenantId_Source_LanguageName_Key", 3);

            modelBuilder.Entity<ApplicationLanguageText>()
                .Property(e => e.Key)
                .CreateIndex("IX_TenantId_Source_LanguageName_Key", 4);

            #endregion

            #region NotificationSubscriptionInfo.IX_NotificationName_EntityTypeName_EntityId_UserId

            modelBuilder.Entity<NotificationSubscriptionInfo>()
                .Property(e => e.NotificationName)
                .CreateIndex("IX_NotificationName_EntityTypeName_EntityId_UserId", 1);

            modelBuilder.Entity<NotificationSubscriptionInfo>()
                .Property(e => e.EntityTypeName)
                .CreateIndex("IX_NotificationName_EntityTypeName_EntityId_UserId", 2);

            modelBuilder.Entity<NotificationSubscriptionInfo>()
                .Property(e => e.EntityId)
                .CreateIndex("IX_NotificationName_EntityTypeName_EntityId_UserId", 3);

            modelBuilder.Entity<NotificationSubscriptionInfo>()
                .Property(e => e.UserId)
                .CreateIndex("IX_NotificationName_EntityTypeName_EntityId_UserId", 4);

            #endregion

            #region NotificationSubscriptionInfo.IX_TenantId_NotificationName_EntityTypeName_EntityId_UserId 

            modelBuilder.Entity<NotificationSubscriptionInfo>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_NotificationName_EntityTypeName_EntityId_UserId", 1);

            modelBuilder.Entity<NotificationSubscriptionInfo>()
                .Property(e => e.NotificationName)
                .CreateIndex("IX_TenantId_NotificationName_EntityTypeName_EntityId_UserId", 2);

            modelBuilder.Entity<NotificationSubscriptionInfo>()
                .Property(e => e.EntityTypeName)
                .CreateIndex("IX_TenantId_NotificationName_EntityTypeName_EntityId_UserId", 3);

            modelBuilder.Entity<NotificationSubscriptionInfo>()
                .Property(e => e.EntityId)
                .CreateIndex("IX_TenantId_NotificationName_EntityTypeName_EntityId_UserId", 4);

            modelBuilder.Entity<NotificationSubscriptionInfo>()
                .Property(e => e.UserId)
                .CreateIndex("IX_TenantId_NotificationName_EntityTypeName_EntityId_UserId", 5);

            #endregion

            #region UserNotificationInfo.IX_UserId_State_CreationTime

            modelBuilder.Entity<UserNotificationInfo>()
                .Property(e => e.UserId)
                .CreateIndex("IX_UserId_State_CreationTime", 1);

            modelBuilder.Entity<UserNotificationInfo>()
                .Property(e => e.State)
                .CreateIndex("IX_UserId_State_CreationTime", 2);

            modelBuilder.Entity<UserNotificationInfo>()
                .Property(e => e.CreationTime)
                .CreateIndex("IX_UserId_State_CreationTime", 3);

            #endregion

            #region OrganizationUnit.IX_TenantId_Code

            modelBuilder.Entity<OrganizationUnit>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_Code", 1);

            modelBuilder.Entity<OrganizationUnit>()
                .Property(e => e.Code)
                .CreateIndex("IX_TenantId_Code", 2);

            #endregion

            #region PermissionSetting.IX_TenantId_Name

            modelBuilder.Entity<PermissionSetting>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_Name", 1);

            modelBuilder.Entity<PermissionSetting>()
                .Property(e => e.Name)
                .CreateIndex("IX_TenantId_Name", 2);

            #endregion

            #region RoleClaim.IX_RoleId

            modelBuilder.Entity<RoleClaim>()
                .Property(e => e.RoleId)
                .CreateIndex("IX_RoleId", 1);

            #endregion

            #region RoleClaim.IX_TenantId_ClaimType

            modelBuilder.Entity<RoleClaim>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_ClaimType", 1);

            modelBuilder.Entity<RoleClaim>()
                .Property(e => e.ClaimType)
                .CreateIndex("IX_TenantId_ClaimType", 2);

            #endregion

            #region Role.IX_TenantId_NormalizedName

            modelBuilder.Entity<TRole>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_NormalizedName", 1);

            modelBuilder.Entity<TRole>()
                .Property(e => e.NormalizedName)
                .CreateIndex("IX_TenantId_NormalizedName", 2);

            #endregion

            #region Setting.IX_TenantId_Name

            modelBuilder.Entity<Setting>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_Name", 1);

            modelBuilder.Entity<Setting>()
                .Property(e => e.Name)
                .CreateIndex("IX_TenantId_Name", 2);

            #endregion

            #region TenantNotificationInfo.IX_TenantId

            modelBuilder.Entity<TenantNotificationInfo>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_Name", 1);

            #endregion

            #region UserClaim.IX_TenantId_ClaimType

            modelBuilder.Entity<UserClaim>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_ClaimType", 1);

            modelBuilder.Entity<UserClaim>()
                .Property(e => e.ClaimType)
                .CreateIndex("IX_TenantId_ClaimType", 2);

            #endregion

            #region UserLoginAttempt.IX_TenancyName_UserNameOrEmailAddress_Result

            modelBuilder.Entity<UserLoginAttempt>()
                .Property(e => e.TenancyName)
                .CreateIndex("IX_TenancyName_UserNameOrEmailAddress_Result", 1);

            modelBuilder.Entity<UserLoginAttempt>()
                .Property(e => e.UserNameOrEmailAddress)
                .CreateIndex("IX_TenancyName_UserNameOrEmailAddress_Result", 2);

            modelBuilder.Entity<UserLoginAttempt>()
                .Property(ula => ula.Result)
                .CreateIndex("IX_TenancyName_UserNameOrEmailAddress_Result", 3);

            #endregion

            #region UserLoginAttempt.IX_UserId_TenantId

            modelBuilder.Entity<UserLoginAttempt>()
                .Property(e => e.UserId)
                .CreateIndex("IX_UserId_TenantId", 1);

            modelBuilder.Entity<UserLoginAttempt>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_UserId_TenantId", 2);

            #endregion

            #region UserLogin.IX_TenantId_LoginProvider_ProviderKey

            modelBuilder.Entity<UserLogin>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_LoginProvider_ProviderKey", 1);

            modelBuilder.Entity<UserLogin>()
                .Property(e => e.LoginProvider)
                .CreateIndex("IX_TenantId_LoginProvider_ProviderKey", 2);

            modelBuilder.Entity<UserLogin>()
                .Property(e => e.ProviderKey)
                .CreateIndex("IX_TenantId_LoginProvider_ProviderKey", 3);

            #endregion

            #region UserLogin.IX_TenantId_UserId

            modelBuilder.Entity<UserLogin>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_UserId", 1);

            modelBuilder.Entity<UserLogin>()
                .Property(e => e.UserId)
                .CreateIndex("IX_TenantId_UserId", 2);

            #endregion

            #region UserOrganizationUnit.IX_TenantId_UserId

            modelBuilder.Entity<UserOrganizationUnit>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_UserId", 1);

            modelBuilder.Entity<UserOrganizationUnit>()
                .Property(e => e.UserId)
                .CreateIndex("IX_TenantId_UserId", 2);

            #endregion

            #region UserOrganizationUnit.IX_TenantId_OrganizationUnitId

            modelBuilder.Entity<UserOrganizationUnit>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_OrganizationUnitId", 1);

            modelBuilder.Entity<UserOrganizationUnit>()
                .Property(e => e.OrganizationUnitId)
                .CreateIndex("IX_TenantId_OrganizationUnitId", 2);

            #endregion

            #region OrganizationUnitRole.IX_TenantId_RoleId

            modelBuilder.Entity<OrganizationUnitRole>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_RoleId", 1);

            modelBuilder.Entity<OrganizationUnitRole>()
                .Property(e => e.RoleId)
                .CreateIndex("IX_TenantId_RoleId", 2);

            #endregion

            #region OrganizationUnitRole.IX_TenantId_OrganizationUnitId

            modelBuilder.Entity<OrganizationUnitRole>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_OrganizationUnitId", 1);

            modelBuilder.Entity<OrganizationUnitRole>()
                .Property(e => e.OrganizationUnitId)
                .CreateIndex("IX_TenantId_OrganizationUnitId", 2);

            #endregion

            #region UserRole.IX_TenantId_UserId

            modelBuilder.Entity<UserRole>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_UserId", 1);

            modelBuilder.Entity<UserRole>()
                .Property(e => e.UserId)
                .CreateIndex("IX_TenantId_UserId", 2);

            modelBuilder.Entity<Setting>()
                .HasIndex(e => new { e.TenantId, e.Name, e.UserId })
                .IsUnique();

            #endregion

            #region UserRole.IX_TenantId_RoleId

            modelBuilder.Entity<UserRole>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_RoleId", 1);

            modelBuilder.Entity<UserRole>()
                .Property(e => e.RoleId)
                .CreateIndex("IX_TenantId_RoleId", 2);

            #endregion

            #region TUser.IX_TenantId_NormalizedUserName

            modelBuilder.Entity<TUser>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_NormalizedUserName", 1);

            modelBuilder.Entity<TUser>()
                .Property(e => e.NormalizedUserName)
                .CreateIndex("IX_TenantId_NormalizedUserName", 2);

            #endregion

            #region TUser.IX_TenantId_NormalizedEmailAddress

            modelBuilder.Entity<TUser>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_NormalizedEmailAddress", 1);

            modelBuilder.Entity<TUser>()
                .Property(e => e.NormalizedEmailAddress)
                .CreateIndex("IX_TenantId_NormalizedEmailAddress", 2);

            #endregion

            #region UserToken.IX_TenantId_UserId

            modelBuilder.Entity<UserToken>()
                .Property(e => e.TenantId)
                .CreateIndex("IX_TenantId_UserId", 1);

            modelBuilder.Entity<UserToken>()
                .Property(e => e.UserId)
                .CreateIndex("IX_TenantId_UserId", 2);

            #endregion
        }
    }
}