using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AbpAspNetCoreDemo.Db
{
    public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
    {
        public MyDbContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) //TODO: .SetBasePath(options.ContentRootPath) ???
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
