using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Abp.EntityFrameworkCore.Configuration
{
    public class AbpDbContextConfiguration<TDbContext>
        where TDbContext : DbContext
    {
        public string ConnectionString {get; internal set; }

        public DbConnection Connection { get; internal set; }

        public DbContextOptionsBuilder<TDbContext> DbContextOptions { get; }

        public AbpDbContextConfiguration(string connectionString)
        {
            ConnectionString = connectionString;
            DbContextOptions = new DbContextOptionsBuilder<TDbContext>();
        }

        public AbpDbContextConfiguration(DbConnection connection)
        {
            Connection = connection;
            DbContextOptions = new DbContextOptionsBuilder<TDbContext>();
        }
    }
}