using Abp.DapperCore.Tests.Entities;
using Microsoft.EntityFrameworkCore;
using Abp.EntityFrameworkCore;

namespace Abp.DapperCore.Tests
{
    public class SampleApplicationDbContext2 : AbpDbContext
    {
        public SampleApplicationDbContext2(DbContextOptions<SampleApplicationDbContext2> options)
            : base(options)
        {
        }

        public virtual DbSet<ProductDetail> ProductDetails { get; set; }
    }
}
