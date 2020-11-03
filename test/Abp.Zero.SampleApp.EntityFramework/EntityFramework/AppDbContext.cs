using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using Abp.Zero.EntityFramework;
using Abp.Zero.SampleApp.BookStore;
using Abp.Zero.SampleApp.EntityHistory;
using Abp.Zero.SampleApp.MultiTenancy;
using Abp.Zero.SampleApp.Roles;
using Abp.Zero.SampleApp.TPH;
using Abp.Zero.SampleApp.Users;

namespace Abp.Zero.SampleApp.EntityFramework
{
    public class AppDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        public DbSet<Book> Books { get; set; }

        public DbSet<Advertisement> Advertisements { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DbSet<UserTestEntity> UserTestEntities { get; set; }

        public DbSet<Person> Persons { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<StudentLectureNote> StudentLectureNotes { get; set; }

        public DbSet<CitizenshipInformation> CitizenshipInformation { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Foo> Foo { get; set; }
        
        public AppDbContext(DbConnection existingConnection)
            : base(existingConnection, true)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>().HasRequired(e => e.Post).WithMany(e => e.Comments);

            modelBuilder.Entity<Book>().ToTable("Books");
            modelBuilder.Entity<Book>().Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Store>().Property(e => e.Id).HasColumnName("StoreId");
        }
    }
}
