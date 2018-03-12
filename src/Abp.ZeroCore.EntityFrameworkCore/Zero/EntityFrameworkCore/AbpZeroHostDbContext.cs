using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.BackgroundJobs;
using Abp.MultiTenancy;
using Abp.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Abp.Zero.EntityFrameworkCore
{
    [MultiTenancySide(MultiTenancySides.Host)]
    public abstract class AbpZeroHostDbContext<TTenant, TRole, TUser, TSelf> : AbpZeroCommonDbContext<TRole, TUser, TSelf>
        where TTenant : AbpTenant<TUser>
        where TRole : AbpRole<TUser>
        where TUser : AbpUser<TUser>
        where TSelf : AbpZeroHostDbContext<TTenant, TRole, TUser, TSelf>
    {
        /// <summary>
        /// Tenants
        /// </summary>
        public virtual DbSet<TTenant> Tenants { get; set; }

        /// <summary>
        /// Editions.
        /// </summary>
        public virtual DbSet<Edition> Editions { get; set; }

        /// <summary>
        /// FeatureSettings.
        /// </summary>
        public virtual DbSet<FeatureSetting> FeatureSettings { get; set; }

        /// <summary>
        /// TenantFeatureSetting.
        /// </summary>
        public virtual DbSet<TenantFeatureSetting> TenantFeatureSettings { get; set; }

        /// <summary>
        /// EditionFeatureSettings.
        /// </summary>
        public virtual DbSet<EditionFeatureSetting> EditionFeatureSettings { get; set; }

        /// <summary>
        /// Background jobs.
        /// </summary>
        public virtual DbSet<BackgroundJobInfo> BackgroundJobs { get; set; }

        /// <summary>
        /// User accounts
        /// </summary>
        public virtual DbSet<UserAccount> UserAccounts { get; set; }

        /// <summary>
        /// Notifications.
        /// </summary>
        public virtual DbSet<NotificationInfo> Notifications { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        protected AbpZeroHostDbContext(DbContextOptions<TSelf> options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TTenant>(b =>
            {
                b.HasOne(p => p.DeleterUser)
                    .WithMany()
                    .HasForeignKey(p => p.DeleterUserId);

                b.HasOne(p => p.CreatorUser)
                    .WithMany()
                    .HasForeignKey(p => p.CreatorUserId);

                b.HasOne(p => p.LastModifierUser)
                    .WithMany()
                    .HasForeignKey(p => p.LastModifierUserId);

                b.HasIndex(e => e.TenancyName);
            });

            modelBuilder.Entity<BackgroundJobInfo>(b =>
            {
                b.HasIndex(e => new { e.IsAbandoned, e.NextTryTime });
            });

            modelBuilder.Entity<TenantFeatureSetting>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.Name });
            });

            modelBuilder.Entity<EditionFeatureSetting>(b =>
            {
                b.HasIndex(e => new { e.EditionId, e.Name });
            });

            modelBuilder.Entity<UserAccount>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.UserName });
                b.HasIndex(e => new { e.TenantId, e.EmailAddress });
                b.HasIndex(e => new { e.UserName });
                b.HasIndex(e => new { e.EmailAddress });
            });
        }
    }
}