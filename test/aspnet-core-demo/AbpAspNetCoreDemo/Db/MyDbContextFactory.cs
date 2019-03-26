using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace AbpAspNetCoreDemo.Db
{
    public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
    {
        public MyDbContext CreateDbContext(string[] args)
        {
            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            var opts = new DbContextOptionsBuilder<MyDbContext>()
                .UseSqlite(inMemorySqlite)
                .Options;

            var dbContext = new MyDbContext(opts);

            return dbContext;
        }
    }
}
