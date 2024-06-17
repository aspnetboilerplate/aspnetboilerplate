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
    protected AbpEfCoreCurrentDbContext AbpEfCoreCurrentDbContext { get; }

    public AbpCompiledQueryCacheKeyGenerator(
        ICompiledQueryCacheKeyGenerator innerCompiledQueryCacheKeyGenerator,
        ICurrentDbContext currentContext,
        AbpEfCoreCurrentDbContext abpEfCoreCurrentDbContext)
    {
        InnerCompiledQueryCacheKeyGenerator = innerCompiledQueryCacheKeyGenerator;
        CurrentContext = currentContext;
        AbpEfCoreCurrentDbContext = abpEfCoreCurrentDbContext;
    }

    public virtual object GenerateCacheKey(Expression query, bool async)
    {
        var cacheKey = InnerCompiledQueryCacheKeyGenerator.GenerateCacheKey(query, async);
        if (CurrentContext.Context is AbpDbContext abpDbContext)
        {
            AbpEfCoreCurrentDbContext.Current.Value = abpDbContext;

            var currentTenantId = abpDbContext.CurrentTenantId;
            var currentFilterStatus = $"{abpDbContext.IsSoftDeleteFilterEnabled}:{abpDbContext.IsMayHaveTenantFilterEnabled}:{abpDbContext.IsMustHaveTenantFilterEnabled}";
            return new AbpCompiledQueryCacheKey(cacheKey, currentTenantId, currentFilterStatus);
        }

        return cacheKey;
    }

    private readonly struct AbpCompiledQueryCacheKey : IEquatable<AbpCompiledQueryCacheKey>
    {
        private readonly object _compiledQueryCacheKey;
        private readonly int? _currentTenantId;
        private readonly string _currentFilterStatus;

        public AbpCompiledQueryCacheKey(object compiledQueryCacheKey, int? currentTenantId, string currentFilterStatus)
        {
            _compiledQueryCacheKey = compiledQueryCacheKey;
            _currentTenantId = currentTenantId;
            _currentFilterStatus = currentFilterStatus;
        }

        public override bool Equals(object obj)
        {
            return obj is AbpCompiledQueryCacheKey key && Equals(key);
        }

        public bool Equals(AbpCompiledQueryCacheKey other)
        {
            return _compiledQueryCacheKey.Equals(other._compiledQueryCacheKey) &&
                   _currentTenantId == other._currentTenantId &&
                   _currentFilterStatus == other._currentFilterStatus;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_compiledQueryCacheKey, _currentTenantId, _currentFilterStatus);
        }
    }
}
