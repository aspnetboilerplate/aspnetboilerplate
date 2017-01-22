using Abp.EntityFrameworkCore;
using AbpAspNetCoreDemo.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace AbpAspNetCoreDemo.Db
{
    public class MyDbContext : AbpDbContext
    {
        public DbSet<Product> Products { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
    }
}