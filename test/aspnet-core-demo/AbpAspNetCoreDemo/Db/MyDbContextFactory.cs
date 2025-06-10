using Abp.EntityFrameworkCore.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AbpAspNetCoreDemo.Db;

public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
        var builder = new DbContextOptionsBuilder<MyDbContext>().UseSqlite(inMemorySqlite).AddAbpDbContextOptionsExtension();

        var dbContext = new MyDbContext(builder.Options);

        return dbContext;
    }
}