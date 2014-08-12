using System.Data.Entity;
using Abp.Application.Authorization.Permissions;
using Abp.Application.Authorization.Roles;
using Abp.Configuration;
using Abp.Domain.Repositories.EntityFramework;
using Abp.MultiTenancy;
using Abp.Users;

namespace Abp.Zero.Repositories.EntityFramework
{
    public class CoreModuleDbContext : AbpDbContext
    {
        public virtual IDbSet<AbpUser> AbpUsers { get; set; }
        
        public virtual IDbSet<UserLogin> UserLogins { get; set; }
        
        public virtual IDbSet<AbpRole> AbpRoles { get; set; }
        
        public virtual IDbSet<UserRole> UserRoles { get; set; }
        
        public virtual IDbSet<AbpTenant> AbpTenants { get; set; }
        
        public virtual IDbSet<Permission> Permissions { get; set; }
        
        public virtual IDbSet<Setting> Settings { get; set; }

        public CoreModuleDbContext()
            : base("Taskever")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AbpUser>().ToTable("AbpUsers");
            modelBuilder.Entity<UserLogin>().ToTable("AbpUserLogins");
            modelBuilder.Entity<AbpRole>().ToTable("AbpRoles");
            modelBuilder.Entity<UserRole>().ToTable("AbpUserRoles");
            modelBuilder.Entity<AbpTenant>().ToTable("AbpTenants");
            modelBuilder.Entity<Permission>().ToTable("AbpPermissions");
            modelBuilder.Entity<Setting>().ToTable("AbpSettings");
        }
    }
}