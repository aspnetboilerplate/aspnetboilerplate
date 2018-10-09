using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using Abp.Zero.EntityFramework;
using Abp.Zero.SampleApp.BookStore;
using Abp.Zero.SampleApp.MultiTenancy;
using Abp.Zero.SampleApp.Roles;
using Abp.Zero.SampleApp.Users;

namespace Abp.Zero.SampleApp.EntityFramework
{
    public class AppDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        public DbSet<Book> Books { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Store> Stores { get; set; }

        public AppDbContext(DbConnection existingConnection)
            : base(existingConnection, true)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>().ToTable("Books");
            modelBuilder.Entity<Book>().Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Store>().Property(e => e.Id).HasColumnName("StoreId");
        }
    }
}