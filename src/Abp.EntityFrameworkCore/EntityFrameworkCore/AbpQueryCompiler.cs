using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Abp.EntityFrameworkCore
{
#pragma warning disable EF1001 // Internal EF Core API usage.
    public class AbpQueryCompiler : QueryCompiler
    {
        private readonly IQueryContextFactory _queryContextFactory;
        private readonly ICompiledQueryCache _compiledQueryCache;
        private readonly ICompiledQueryCacheKeyGenerator _compiledQueryCacheKeyGenerator;
        private readonly IDatabase _database;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _logger;

        private readonly IEvaluatableExpressionFilter _evaluatableExpressionFilter;
        private readonly IModel _model;

        private readonly ICurrentDbContext _currentDbContext;

        public AbpQueryCompiler(
            IQueryContextFactory queryContextFactory,
            ICompiledQueryCache compiledQueryCache,
            ICompiledQueryCacheKeyGenerator compiledQueryCacheKeyGenerator,
            IDatabase database,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger,
            ICurrentDbContext currentContext,
            IEvaluatableExpressionFilter evaluatableExpressionFilter,
            IModel model) :
            base(queryContextFactory, compiledQueryCache, compiledQueryCacheKeyGenerator, database, logger, currentContext, evaluatableExpressionFilter, model)
        {
            _queryContextFactory = queryContextFactory;
            _compiledQueryCache = compiledQueryCache;
            _compiledQueryCacheKeyGenerator = compiledQueryCacheKeyGenerator;
            _database = database;
            _logger = logger;
            _evaluatableExpressionFilter = evaluatableExpressionFilter;
            _model = model;
            _currentDbContext = currentContext;
        }

        public override TResult Execute<TResult>(Expression query) => ExecuteCore<TResult>(query, async: false, CancellationToken.None);

        public override TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken = default) => ExecuteCore<TResult>(query, async: true, cancellationToken);

        private TResult ExecuteCore<TResult>(Expression query, bool async, CancellationToken cancellationToken)
        {
            var queryContext = _queryContextFactory.Create();

            queryContext.CancellationToken = cancellationToken;

            var queryAfterExtraction = ExtractParameters(query, queryContext, _logger);

            var _defaultCompiledQueryCacheKey = _compiledQueryCacheKeyGenerator.GenerateCacheKey(queryAfterExtraction, async);
            var globalFilters = (string)null;
            if (_currentDbContext.Context is AbpDbContext abpDbContext)
            {
                var list = abpDbContext.CurrentUnitOfWorkProvider?.Current?.Filters?.Select(s => $"{s.FilterName}:{s.IsEnabled}");
                globalFilters = list == null ? "" : string.Join("|", list);
            }
            var compiledQueryCacheKey = new CompiledQueryCacheKey(_defaultCompiledQueryCacheKey, globalFilters);

            var compiledQuery
                = _compiledQueryCache
                    .GetOrAddQuery(
                        compiledQueryCacheKey,
                        () => RuntimeFeature.IsDynamicCodeSupported
                            ? CompileQueryCore<TResult>(_database, queryAfterExtraction, _model, async)
                            : throw new InvalidOperationException("Query wasn't precompiled and dynamic code isn't supported (NativeAOT)"));

            return compiledQuery(queryContext);
        }

        private readonly struct CompiledQueryCacheKey : IEquatable<CompiledQueryCacheKey>
        {
            private readonly object _defaultCompiledQueryCacheKey;
            private readonly string _globalFilters;

            public CompiledQueryCacheKey(
                object defaultCompiledQueryCacheKey,
                string globalFilters)
            {
                _defaultCompiledQueryCacheKey = defaultCompiledQueryCacheKey;
                _globalFilters = globalFilters;
            }

            public override bool Equals(object obj)
            {
                return obj is CompiledQueryCacheKey compiledQueryCacheKey && Equals(compiledQueryCacheKey);
            }

            public bool Equals(CompiledQueryCacheKey other)
            {
                return _defaultCompiledQueryCacheKey.Equals(other._defaultCompiledQueryCacheKey) && _globalFilters == other._globalFilters;
            }

            public override int GetHashCode()
                => HashCode.Combine(_defaultCompiledQueryCacheKey, _globalFilters);
        }
    }
#pragma warning restore EF1001 // Internal EF Core API usage.
}
