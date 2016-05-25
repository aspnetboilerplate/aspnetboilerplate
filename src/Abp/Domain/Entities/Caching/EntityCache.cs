using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;

namespace Abp.Domain.Entities.Caching
{
    public abstract class EntityCache<TEntity, TCacheItem> :
        EntityCache<TEntity, TCacheItem, int>,
        IEventHandler<EntityChangedEventData<TEntity>>, IEntityCache<TCacheItem>
        where TEntity : class, IEntity<int>
    {
        protected EntityCache(
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

    public abstract class EntityCache<TEntity, TCacheItem, TPrimaryKey> :
        IEventHandler<EntityChangedEventData<TEntity>>, IEntityCache<TCacheItem, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public TCacheItem this[TPrimaryKey id]
        {
            get { return Get(id); }
        }

        public string CacheName { get; private set; }

        public ITypedCache<TPrimaryKey, TCacheItem> InternalCache
        {
            get
            {
                return CacheManager.GetCache<TPrimaryKey, TCacheItem>(CacheName);
            }
        }

        protected ICacheManager CacheManager { get; private set; }

        protected IRepository<TEntity, TPrimaryKey> Repository { get; private set; }

        protected EntityCache(ICacheManager cacheManager, IRepository<TEntity, TPrimaryKey> repository, string cacheName = null)
        {
            Repository = repository;
            CacheManager = cacheManager;
            CacheName = cacheName ?? GenerateDefaultCacheName();
        }

        public TCacheItem Get(TPrimaryKey id)
        {
            return InternalCache.Get(id, () => GetCacheItemFromDataSource(id));
        }

        public Task<TCacheItem> GetAsync(TPrimaryKey id)
        {
            return InternalCache.GetAsync(id, () => GetCacheItemFromDataSourceAsync(id));
        }

        public void HandleEvent(EntityChangedEventData<TEntity> eventData)
        {
            InternalCache.Remove(eventData.Entity.Id);
        }

        protected virtual TCacheItem GetCacheItemFromDataSource(TPrimaryKey id)
        {
            return MapToCacheItem(GetEntityFromDataSource(id));
        }

        protected virtual async Task<TCacheItem> GetCacheItemFromDataSourceAsync(TPrimaryKey id)
        {
            return MapToCacheItem(await GetEntityFromDataSourceAsync(id));
        }

        protected virtual TEntity GetEntityFromDataSource(TPrimaryKey id)
        {
            return Repository.FirstOrDefault(id);
        }

        protected virtual Task<TEntity> GetEntityFromDataSourceAsync(TPrimaryKey id)
        {
            return Repository.FirstOrDefaultAsync(id);
        }

        protected abstract TCacheItem MapToCacheItem(TEntity entity);

        protected virtual string GenerateDefaultCacheName()
        {
            return GetType().FullName;
        }

        public override string ToString()
        {
            return string.Format("EntityCache {0}", CacheName);
        }
    }
}
