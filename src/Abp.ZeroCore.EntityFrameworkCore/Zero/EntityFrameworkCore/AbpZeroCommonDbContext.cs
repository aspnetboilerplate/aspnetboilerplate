using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.EntityFrameworkCore;
using Abp.EntityHistory;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.Zero.EntityFrameworkCore
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
        /// Entity changes.
        /// </summary>
        public virtual DbSet<EntityChange> EntityChanges { get; set; }

        /// <summary>
        /// Entity change sets.
        /// </summary>
        public virtual DbSet<EntityChangeSet> EntityChangeSets { get; set; }

        /// <summary>
        /// Entity property changes.
        /// </summary>
        public virtual DbSet<EntityPropertyChange> EntityPropertyChanges { get; set; }

        public IEntityHistoryHelper EntityHistoryHelper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        protected AbpZeroCommonDbContext(DbContextOptions<TSelf> options)
            : base(options)
        {

        }

        public override int SaveChanges()
        {
            var changeSet = EntityHistoryHelper?.CreateEntityChangeSet(ChangeTracker.Entries().ToList());

            var result = base.SaveChanges();

            EntityHistoryHelper?.Save(changeSet);

            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var changeSet = EntityHistoryHelper?.CreateEntityChangeSet(ChangeTracker.Entries().ToList());

            var result = await base.SaveChangesAsync(cancellationToken);

            if (EntityHistoryHelper != null)
            {
                await EntityHistoryHelper.SaveAsync(changeSet);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUser>(b =>
            {
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                b.HasOne(p => p.DeleterUser)
                    .WithMany()
                    .HasForeignKey(p => p.DeleterUserId);

                b.HasOne(p => p.CreatorUser)
                    .WithMany()
                    .HasForeignKey(p => p.CreatorUserId);

                b.HasOne(p => p.LastModifierUser)
                    .WithMany()
                    .HasForeignKey(p => p.LastModifierUserId);
            });

            modelBuilder.Entity<TRole>(b =>
            {
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();
            });

            modelBuilder.Entity<AuditLog>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.ExecutionTime });
                b.HasIndex(e => new { e.TenantId, e.ExecutionDuration });
            });

            modelBuilder.Entity<ApplicationLanguage>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.Name });
            });

            modelBuilder.Entity<ApplicationLanguageText>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.Source, e.LanguageName, e.Key });
            });

            modelBuilder.Entity<EntityChange>(b =>
            {
                b.HasMany(p => p.PropertyChanges)
                    .WithOne()
                    .HasForeignKey(p => p.EntityChangeId);

                b.HasIndex(e => new { e.EntityChangeSetId });
                b.HasIndex(e => new { e.EntityTypeFullName, e.EntityId });
            });

            modelBuilder.Entity<EntityChangeSet>(b =>
            {
                b.HasMany(p => p.EntityChanges)
                    .WithOne()
                    .HasForeignKey(p => p.EntityChangeSetId);

                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.CreationTime });
                b.HasIndex(e => new { e.TenantId, e.Reason });
            });

            modelBuilder.Entity<EntityPropertyChange>(b =>
            {
                b.HasIndex(e => e.EntityChangeId);
            });

            modelBuilder.Entity<NotificationSubscriptionInfo>(b =>
            {
                b.HasIndex(e => new { e.NotificationName, e.EntityTypeName, e.EntityId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.NotificationName, e.EntityTypeName, e.EntityId, e.UserId });
            });

            modelBuilder.Entity<OrganizationUnit>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.Code });
            });

            modelBuilder.Entity<PermissionSetting>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.Name });
            });

            modelBuilder.Entity<RoleClaim>(b =>
            {
                b.HasIndex(e => new { e.RoleId });
                b.HasIndex(e => new { e.TenantId, e.ClaimType });
            });

            modelBuilder.Entity<TRole>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.NormalizedName });
            });

            modelBuilder.Entity<Setting>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.Name, e.UserId }).IsUnique().HasFilter(null);
            });

            modelBuilder.Entity<TenantNotificationInfo>(b =>
            {
                b.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.Entity<UserClaim>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.ClaimType });
            });

            modelBuilder.Entity<UserLoginAttempt>(b =>
            {
                b.HasIndex(e => new { e.TenancyName, e.UserNameOrEmailAddress, e.Result });
                b.HasIndex(ula => new { ula.UserId, ula.TenantId });
            });

            modelBuilder.Entity<UserLogin>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.LoginProvider, e.ProviderKey });
                b.HasIndex(e => new { e.TenantId, e.UserId });
            });

            modelBuilder.Entity<UserNotificationInfo>(b =>
            {
                b.HasIndex(e => new { e.UserId, e.State, e.CreationTime });
            });

            modelBuilder.Entity<UserOrganizationUnit>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.OrganizationUnitId });
            });

            modelBuilder.Entity<OrganizationUnitRole>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.RoleId });
                b.HasIndex(e => new { e.TenantId, e.OrganizationUnitId });
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.RoleId });
            });

            modelBuilder.Entity<TUser>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.NormalizedUserName });
                b.HasIndex(e => new { e.TenantId, e.NormalizedEmailAddress });
            });

            modelBuilder.Entity<UserToken>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
            });
        }
    }
}