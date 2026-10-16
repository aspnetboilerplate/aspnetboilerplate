using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Abp.EntityFrameworkCore.Tests.Ef;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace Abp.EntityFrameworkCore.Tests.Tests;

public class CompiledQueryTestHooks
{
    public Action<QueryCacheDbContext> OnQueryCompilationStarting { get; set; }

    public Action OnGetCompiledQueryCacheKey { get; set; }
}

public class CompiledQueryTestInterceptor : IQueryExpressionInterceptor
{
    private readonly CompiledQueryTestHooks _hooks;

    public CompiledQueryTestInterceptor(CompiledQueryTestHooks hooks)
    {
        _hooks = hooks;
    }

    public Expression QueryCompilationStarting(Expression queryExpression, QueryExpressionEventData eventData)
    {
        if (eventData.Context is QueryCacheDbContext dbContext)
        {
            _hooks.OnQueryCompilationStarting?.Invoke(dbContext);
        }

        return queryExpression;
    }
}

// A custom database that an application registers with ReplaceService or in its own EF Core service provider.
public class CompiledQueryTestDatabase : RelationalDatabase, IDisposable
{
    public int SaveChangesCount { get; private set; }

    public int DisposeCount { get; private set; }

    public CompiledQueryTestDatabase(DatabaseDependencies dependencies, RelationalDatabaseDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

    public override int SaveChanges(IList<IUpdateEntry> entries)
    {
        SaveChangesCount++;
        return base.SaveChanges(entries);
    }

    public override Task<int> SaveChangesAsync(IList<IUpdateEntry> entries, CancellationToken cancellationToken = default)
    {
        SaveChangesCount++;
        return base.SaveChangesAsync(entries, cancellationToken);
    }

    public void Dispose()
    {
        DisposeCount++;
    }
}
