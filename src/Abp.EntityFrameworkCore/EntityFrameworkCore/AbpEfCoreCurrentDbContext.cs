using System;
using System.Linq.Expressions;
using System.Threading;

namespace Abp.EntityFrameworkCore;

public class AbpEfCoreCurrentDbContext
{
    private readonly AsyncLocal<AbpDbContext> _current = new AsyncLocal<AbpDbContext>();
    private readonly AsyncLocal<QueryScope> _queryScope = new AsyncLocal<QueryScope>();

    public AbpDbContext Context => _current.Value;

    public virtual AbpDbContext GetRequiredContext()
    {
        return Context ?? throw new AbpException(
            "The data filters cannot be applied to a query that is compiled outside of EF Core's normal query pipeline, such as EF.CompileQuery or EF.CompileAsyncQuery. " +
            "Run it as a normal LINQ query, or set 'UseAbpQueryCompiler' of 'IAbpEfCoreConfiguration' to false.");
    }

    public IDisposable Use(AbpDbContext context)
    {
        var previousValue = Context;
        _current.Value = context;
        return new DisposeAction(() =>
        {
            _current.Value = previousValue;
        });
    }

    // Only the query of AbpEntityQueryProvider puts the filter state into the compiled query cache key, so only its compilation
    // may read it. Other compilations, such as EF.CompileQuery or a query compiled while another query runs, must fail.
    internal IDisposable BeginQuery(AbpDbContext context)
    {
        var previousScope = _queryScope.Value;
        _queryScope.Value = context == null ? null : new QueryScope(context);
        var contextScope = Use(context);
        return new DisposeAction(() =>
        {
            contextScope.Dispose();
            _queryScope.Value = previousScope;
        });
    }

    internal void AuthorizeQueryCompilation(AbpDbContext context, Expression query)
    {
        var scope = _queryScope.Value;
        if (scope != null && scope.Context == context)
        {
            scope.AuthorizedQuery = query;
        }
    }

    internal IDisposable BeginQueryCompilation(AbpDbContext context, Expression query)
    {
        var scope = _queryScope.Value;
        if (scope == null || scope.Context != context || !ReferenceEquals(scope.AuthorizedQuery, query))
        {
            return Use(null);
        }

        scope.AuthorizedQuery = null;
        return Use(scope.Context);
    }

    private sealed class QueryScope
    {
        public AbpDbContext Context { get; }

        public Expression AuthorizedQuery { get; set; }

        public QueryScope(AbpDbContext context)
        {
            Context = context;
        }
    }
}