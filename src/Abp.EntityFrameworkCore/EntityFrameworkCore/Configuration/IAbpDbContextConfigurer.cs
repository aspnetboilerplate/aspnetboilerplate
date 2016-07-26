using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Configuration
{
    public interface IAbpDbContextConfigurer<TDbContext>
        where TDbContext : DbContext
    {
        void Configure(AbpDbContextConfiguration<TDbContext> configuration);
    }
}