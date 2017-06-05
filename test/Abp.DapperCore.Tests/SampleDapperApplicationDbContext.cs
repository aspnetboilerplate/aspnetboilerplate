using Abp.DapperCore.Tests.Entities;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Abp.DapperCore.Tests
{
    public class SampleDapperApplicationDbContext : AbpDbContext
    {
        public SampleDapperApplicationDbContext(DbContextOptions<SampleDapperApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
    }
}
