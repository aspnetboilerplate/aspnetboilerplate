using Abp.EntityFrameworkCore.Tests.Domain;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Tests
{
    public class BloggingDbContext : AbpDbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        public BloggingDbContext(DbContextOptions<BloggingDbContext> options)
            : base(options)
        {
            
        }
    }
}
