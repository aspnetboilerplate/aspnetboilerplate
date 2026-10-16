using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityFrameworkCore.Tests.Tests;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Tests.Ef;

public class QueryCacheDbContext : AbpDbContext
{
    public DbSet<TenantNote> TenantNotes { get; set; }

    public DbSet<TenantOrder> TenantOrders { get; set; }

    public DbSet<TenantMemo> TenantMemos { get; set; }

    public DbSet<IntSoftDeleteNote> IntSoftDeleteNotes { get; set; }

    public CompiledQueryTestHooks CompiledQueryTestHooks { get; set; }

    public QueryCacheDbContext(DbContextOptions<QueryCacheDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IntSoftDeleteNote>()
            .Property(n => n.IsDeleted)
            .HasConversion(
                v => v ? IntSoftDeleteNote.DeletedValue : IntSoftDeleteNote.NotDeletedValue,
                v => v == IntSoftDeleteNote.DeletedValue);
    }

    public override string GetCompiledQueryCacheKey()
    {
        CompiledQueryTestHooks?.OnGetCompiledQueryCacheKey?.Invoke();
        return base.GetCompiledQueryCacheKey();
    }
}
