using Microsoft.EntityFrameworkCore;

namespace Abp.ZeroCore.SampleApp.EntityFramework
{
    public static class AbpZeroTemplateDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<SampleAppDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }
    }
}