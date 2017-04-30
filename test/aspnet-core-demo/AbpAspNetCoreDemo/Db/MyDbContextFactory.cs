using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace AbpAspNetCoreDemo.Db
{
    public class MyDbContextFactory : IDbContextFactory<MyDbContext>
    {
        public MyDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(options.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            var opts = new DbContextOptionsBuilder<MyDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"))
                .Options;

            return new MyDbContext(opts);
        }
    }
}
