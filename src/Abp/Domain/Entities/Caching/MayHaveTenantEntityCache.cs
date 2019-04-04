using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;

namespace Abp.Domain.Entities.Caching
{
    public class MayHaveTenantEntityCache<TEntity, TCacheItem> :
        MayHaveTenantEntityCache<TEntity, TCacheItem, int>,
        IMultiTenanyEntityCache<TCacheItem>
        where TEntity : class, IEntity<int>, IMayHaveTenant
    {
        public MayHaveTenantEntityCache(
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

    public class MayHaveTenantEntityCache<TEntity, TCacheItem, TPrimaryKey> :
        MultiTenancyEntityCache<TEntity, TCacheItem, TPrimaryKey>,
        IEventHandler<EntityChangedEventData<TEntity>>,
        IMultiTenanyEntityCache<TCacheItem, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, IMayHaveTenant
    {
        public MayHaveTenantEntityCache(
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
            return string.Format("MayHaveTenantEntityCache {0}", CacheName);
        }
    }
}
