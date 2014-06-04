using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using Abp.Domain.Repositories.EntityFramework;
using Abp.Security.Users;
using Taskever.Activities;
using Taskever.Friendships;
using Taskever.Security.Roles;
using Taskever.Security.Users;
using Taskever.Tasks;

namespace Taskever.Infrastructure.EntityFramework.Data.Repositories.NHibernate
{
    public class TaskeverDbContext : AbpDbContext
    {
        //public virtual IDbSet<TaskeverUser> TaskeverUser { get; set; }

        //public virtual IDbSet<TaskeverRole> TaskeverRoles { get; set; }

        //public virtual IDbSet<Friendship> Tasks { get; set; }
        //public virtual IDbSet<Task> Tasks { get; set; }

        public TaskeverDbContext()
            : base("Taskever")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //TODO: Ignore base classes

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

            modelBuilder.Ignore<AbpUser>();

            modelBuilder.Entity<TaskeverUser>().ToTable("AbpUsers");
            modelBuilder.Entity<Activity>().ToTable("TeActivities")
                .Map<CreateTaskActivity>(m => m.Requires("ActivityType").HasValue(1))
                .Map<CompleteTaskActivity>(m => m.Requires("ActivityType").HasValue(2));

            //modelBuilder.Entity<CompleteTaskActivity>()
            modelBuilder.Entity<Friendship>().ToTable("TeFriendships");
            modelBuilder.Entity<Task>().ToTable("TeTasks");
            modelBuilder.Entity<UserFollowedActivity>().ToTable("TeUserFollowedActivities");
        }
    }
}