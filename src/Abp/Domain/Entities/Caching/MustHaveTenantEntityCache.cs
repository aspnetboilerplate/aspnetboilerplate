using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;

namespace Abp.Domain.Entities.Caching
{
    public class MustHaveTenantEntityCache<TEntity, TCacheItem> : MustHaveTenantEntityCache<TEntity, TCacheItem, int>,
        IMultiTenancyEntityCache<TCacheItem>
        where TEntity : class, IEntity<int>, IMustHaveTenant
    {
        public MustHaveTenantEntityCache(
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TEntity, int> repository,
            string cacheName = null)
            : base(
                cacheManager,
                unitOfWorkManager,
                repository,
                cacheName)
        {
        }
    }

    public class MustHaveTenantEntityCache<TEntity, TCacheItem, TPrimaryKey> : MultiTenancyEntityCache<TEntity, TCacheItem, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, IMustHaveTenant
    {
        public MustHaveTenantEntityCache(
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TEntity, TPrimaryKey> repository,
            string cacheName = null)
            : base(
                cacheManager,
                unitOfWorkManager,
                repository,
                cacheName)
        {
        }

        protected override string GetCacheKey(TEntity entity)
        {
            return GetCacheKey(entity.Id, entity.TenantId);
        }

        public override string ToString()
        {
            return $"MustHaveTenantEntityCache {CacheName}";
        }
    }
}
