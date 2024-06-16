using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using JetBrains.Annotations;

namespace Abp.EntityFrameworkCore;

#pragma warning disable EF1001 // Internal EF Core API usage.
public class AbpQueryCompiler : QueryCompiler
{
    // We need these fields because some third party libraries(EF Plus) try to get them via reflection.
    private readonly IQueryContextFactory _queryContextFactory;
    private readonly ICompiledQueryCache _compiledQueryCache;
    private readonly ICompiledQueryCacheKeyGenerator _compiledQueryCacheKeyGenerator;
    private readonly IDatabase _database;
    private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _logger;
    private readonly Type _contextType;
    private readonly IEvaluatableExpressionFilter _evaluatableExpressionFilter;
    private readonly IModel _model;

    public AbpQueryCompiler(
        [NotNull] IQueryContextFactory queryContextFactory,
        [NotNull] ICompiledQueryCache compiledQueryCache,
        [NotNull] ICompiledQueryCacheKeyGenerator compiledQueryCacheKeyGenerator,
        [NotNull] IDatabase database,
        [NotNull] IDiagnosticsLogger<DbLoggerCategory.Query> logger,
        [NotNull] ICurrentDbContext currentContext,
        [NotNull] IEvaluatableExpressionFilter evaluatableExpressionFilter,
        [NotNull] IModel model)
        : base(
            queryContextFactory,
            compiledQueryCache,
            new AbpCompiledQueryCacheKeyGenerator(compiledQueryCacheKeyGenerator, currentContext),
            database,
            logger,
            currentContext,
            evaluatableExpressionFilter,
            model)
    {
        _queryContextFactory = queryContextFactory;
        _compiledQueryCache = compiledQueryCache;
        _compiledQueryCacheKeyGenerator = compiledQueryCacheKeyGenerator;
        _database = database;
        _logger = logger;
        _contextType = currentContext.Context.GetType();
        _evaluatableExpressionFilter = evaluatableExpressionFilter;
        _model = model;
    }
}
#pragma warning restore EF1001 // Internal EF Core API usage.
