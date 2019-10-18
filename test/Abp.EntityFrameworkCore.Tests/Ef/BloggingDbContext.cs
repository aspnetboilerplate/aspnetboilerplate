using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityFrameworkCore.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Tests.Ef
{
    public class BloggingDbContext : AbpDbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        public DbSet<BlogView> BlogView { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<BlogCategory> BlogCategories { get; set; }

        public DbSet<SubBlogCategory> SubBlogCategories { get; set; }

        public BloggingDbContext(DbContextOptions<BloggingDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(b =>
            {
                b.OwnsOne(t => t.BlogTime, x =>
                    {
                        x.Property(p => p.LastAccessTime).HasConversion(new AbpDateTimeValueConverter());
                    });
            });

            modelBuilder
                .Entity<BlogView>()
                .HasNoKey()
                .ToView("BlogView");

            base.OnModelCreating(modelBuilder);
        }
    }
}
