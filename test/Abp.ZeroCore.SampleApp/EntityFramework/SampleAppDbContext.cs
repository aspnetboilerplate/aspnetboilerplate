using System.ComponentModel.DataAnnotations.Schema;
using Abp.IdentityServer4;
using Abp.Zero.EntityFrameworkCore;
using Abp.ZeroCore.SampleApp.Core;
using Abp.ZeroCore.SampleApp.Core.BookStore;
using Abp.ZeroCore.SampleApp.Core.EntityHistory;
using Abp.ZeroCore.SampleApp.Core.Shop;
using Microsoft.EntityFrameworkCore;

namespace Abp.ZeroCore.SampleApp.EntityFramework
{
    //TODO: Re-enable when IdentityServer ready
    public class SampleAppDbContext : AbpZeroDbContext<Tenant, Role, User, SampleAppDbContext>, IAbpPersistedGrantDbContext
    {
        public DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public DbSet<Advertisement> Advertisements { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductTranslation> ProductTranslations { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderTranslation> OrderTranslations { get; set; }

        public DbSet<UserTestEntity> UserTestEntities { get; set; }

        public SampleAppDbContext(DbContextOptions<SampleAppDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigurePersistedGrantEntity();

            modelBuilder.Entity<Blog>().OwnsOne(x => x.More);

            modelBuilder.Entity<Book>().ToTable("Books");
            modelBuilder.Entity<Book>().Property(e => e.Id).ValueGeneratedNever();

            modelBuilder.Entity<Store>().Property(e => e.Id).HasColumnName("StoreId");
        }
    }
}
