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

    }
}
#pragma warning restore EF1001 // Internal EF Core API usage.
