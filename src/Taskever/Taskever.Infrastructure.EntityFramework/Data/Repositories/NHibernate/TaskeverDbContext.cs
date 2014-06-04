using System.Data.Entity;
using Abp.Configuration;
using Abp.Domain.Repositories.EntityFramework;
using Abp.Security.Permissions;
using Abp.Security.Roles;
using Abp.Security.Tenants;
using Abp.Security.Users;
using Taskever.Activities;
using Taskever.Friendships;
using Taskever.Security.Users;
using Taskever.Tasks;

namespace Taskever.Infrastructure.EntityFramework.Data.Repositories.NHibernate
{
    public class TaskeverDbContext : AbpDbContext
    {
        public TaskeverDbContext()
            : base("Taskever")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Permission>().ToTable("AbpPermissions");
            //modelBuilder.Entity<UserRole>().ToTable("AbpUserRoles");
            //modelBuilder.Entity<Setting>().ToTable("AbpSettings");
            //modelBuilder.Entity<AbpRole>().ToTable("AbpRoles");
            //modelBuilder.Entity<AbpTenant>().ToTable("AbpTenants");
            //modelBuilder.Entity<UserLogin>().ToTable("AbpUserLogins");

            //modelBuilder.Entity<UserRole>().ToTable("AbpUserRoles");

            //modelBuilder.Entity<AbpRole>().HasMany(r => r.Permissions).WithRequired().HasForeignKey(p => p.RoleId);

            //modelBuilder.Entity<UserRole>().HasRequired(ur => ur.Role);
            //modelBuilder.Entity<UserRole>().HasRequired(ur => ur.User);
            
            //modelBuilder.Entity<AbpUser>().ToTable("AbpUsers");

            //modelBuilder.Entity<TaskeverUser>().ToTable("AbpUsers");
            //modelBuilder.Entity<Activity>().ToTable("TeActivities");
            //modelBuilder.Entity<Friendship>().ToTable("TeFriendships");
            //modelBuilder.Entity<Task>().ToTable("TeTasks");
            //modelBuilder.Entity<UserFollowedActivity>().ToTable("TeUserFollowedActivities");
        }
    }
}