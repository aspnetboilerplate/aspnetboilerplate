using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Abp.EntityFrameworkCore.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder AddAbpOptionsExtension(this DbContextOptionsBuilder optionsBuilder)
    {
        ((IDbContextOptionsBuilderInfrastructure) optionsBuilder).AddOrUpdateExtension(new AbpOptionsExtension());
        return optionsBuilder;
    }

    public static DbContextOptionsBuilder<TContext> AddAbpOptionsExtension<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder)
        where TContext : DbContext
    {
        ((IDbContextOptionsBuilderInfrastructure) optionsBuilder).AddOrUpdateExtension(new AbpOptionsExtension());
        return optionsBuilder;
    }
}
