using System.Data.Common;
using System.Data.Entity;

using Abp.Dapper.Tests.Entities;
using Abp.EntityFramework;

namespace Abp.Dapper.Tests
{
    public class SampleDapperApplicationDbContext : AbpDbContext
    {
        public SampleDapperApplicationDbContext()
        {
        }

        public SampleDapperApplicationDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public SampleDapperApplicationDbContext(DbConnection connection)
            : base(connection, false)
        {
        }

        public SampleDapperApplicationDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
        }

        public virtual IDbSet<Product> Products { get; set; }

        public virtual IDbSet<ProductDetail> ProductDetails { get; set; }
    }
}
