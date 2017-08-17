using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;

using Abp.Dapper.Tests.Entities;
using Abp.EntityFramework;

namespace Abp.Dapper.Tests.DbContexts
{
    [DbConfigurationType(typeof(DapperDbContextConfiguration2))]
    public class SampleApplicationDbContext2 : AbpDbContext
    {
        public SampleApplicationDbContext2(DbConnection connection)
            : base(connection, false)
        {
        }

        public SampleApplicationDbContext2(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
        }

        public virtual IDbSet<ProductDetail> ProductDetails { get; set; }
    }

    public class DapperDbContextConfiguration2 : DbConfiguration
    {
        public DapperDbContextConfiguration2()
        {
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }
    }
}
