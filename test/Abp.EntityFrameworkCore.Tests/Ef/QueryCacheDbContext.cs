using Abp.EntityFrameworkCore.Tests.Domain;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Tests.Ef;

public class QueryCacheDbContext : AbpDbContext
{
    public DbSet<TenantNote> TenantNotes { get; set; }

    public DbSet<TenantOrder> TenantOrders { get; set; }

    public QueryCacheDbContext(DbContextOptions<QueryCacheDbContext> options)
        : base(options)
    {

    }
}
