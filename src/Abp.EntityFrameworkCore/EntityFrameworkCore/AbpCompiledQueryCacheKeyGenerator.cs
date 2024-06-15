using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;

namespace Abp.EntityFrameworkCore;

public class AbpCompiledQueryCacheKeyGenerator : ICompiledQueryCacheKeyGenerator
{
    protected ICompiledQueryCacheKeyGenerator InnerCompiledQueryCacheKeyGenerator { get; }
    protected ICurrentDbContext CurrentContext { get; }

    public AbpCompiledQueryCacheKeyGenerator(
        ICompiledQueryCacheKeyGenerator innerCompiledQueryCacheKeyGenerator,
        ICurrentDbContext currentContext)
    {
        InnerCompiledQueryCacheKeyGenerator = innerCompiledQueryCacheKeyGenerator;
        CurrentContext = currentContext;
    }

    public virtual object GenerateCacheKey(Expression query, bool async)
    {
        var cacheKey = InnerCompiledQueryCacheKeyGenerator.GenerateCacheKey(query, async);
        if (CurrentContext.Context is AbpDbContext abpDbContext)
        {
            var filter = string.Join("|", abpDbContext.CurrentUnitOfWorkProvider?.Current?.Filters?.Select(x => $"{x.FilterName}:{x.IsEnabled}") ?? Array.Empty<string>());
            return new AbpCompiledQueryCacheKey(cacheKey, filter);
        }

        return cacheKey;
    }

    protected readonly struct AbpCompiledQueryCacheKey : IEquatable<AbpCompiledQueryCacheKey>
    {
        private readonly object _compiledQueryCacheKey;
        private readonly string _currentFilter;

        public AbpCompiledQueryCacheKey(object compiledQueryCacheKey, string currentFilter)
        {
            _compiledQueryCacheKey = compiledQueryCacheKey;
            _currentFilter = currentFilter;
        }

        public override bool Equals(object obj)
        {
            return obj is AbpCompiledQueryCacheKey key && Equals(key);
        }

        public bool Equals(AbpCompiledQueryCacheKey other)
        {
            return _compiledQueryCacheKey.Equals(other._compiledQueryCacheKey) && _currentFilter == other._currentFilter;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_compiledQueryCacheKey, _currentFilter);
        }
    }
}
