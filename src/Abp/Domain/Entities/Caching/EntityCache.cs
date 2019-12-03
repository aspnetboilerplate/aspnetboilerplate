using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;

namespace Abp.Domain.Entities.Caching
{
    public class EntityCache<TEntity, TCacheItem> :
        EntityCache<TEntity, TCacheItem, int>,
        IEntityCache<TCacheItem>
        where TEntity : class, IEntity<int>
    {
        public EntityCache(
            ICacheManager cacheManager,
            IRepository<TEntity, int> repository,
            string cacheName = null)
            : base(
                cacheManager,
                repository,
                cacheName)
        {
        }
    }

    public class EntityCache<TEntity, TCacheItem, TPrimaryKey> :
        EntityCacheBase<TEntity, TCacheItem, TPrimaryKey>,
        IEventHandler<EntityChangedEventData<TEntity>>, 
        IEntityCache<TCacheItem, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public ITypedCache<TPrimaryKey, TCacheItem> InternalCache
        {
            get
            {
                return CacheManager.GetCache<TPrimaryKey, TCacheItem>(CacheName);
            }
        }

        public EntityCache(
            ICacheManager cacheManager, 
            IRepository<TEntity, TPrimaryKey> repository, 
            string cacheName = null)
            : base(
                cacheManager,
                repository,
                cacheName)
        {
        }

        public override TCacheItem Get(TPrimaryKey id)
        {
            return InternalCache.Get(id, () => GetCacheItemFromDataSource(id));
        }

        public override Task<TCacheItem> GetAsync(TPrimaryKey id)
        {
            return InternalCache.GetAsync(id, () => GetCacheItemFromDataSourceAsync(id));
        }

        public virtual void HandleEvent(EntityChangedEventData<TEntity> eventData)
        {
            InternalCache.Remove(eventData.Entity.Id);
        }

        public override string ToString()
        {
            return $"EntityCache {CacheName}";
        }
    }
}
