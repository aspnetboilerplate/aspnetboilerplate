using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Caching;

namespace Abp.Domain.Entities.Caching
{
    public abstract class EntityCacheBase<TEntity, TCacheItem, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public TCacheItem this[TPrimaryKey id] => Get(id);

        public string CacheName { get; private set; }

        public IObjectMapper ObjectMapper { get; set; }

        protected ICacheManager CacheManager { get; private set; }

        protected IRepository<TEntity, TPrimaryKey> Repository { get; private set; }

        public EntityCacheBase(
            ICacheManager cacheManager, 
            IRepository<TEntity, TPrimaryKey> repository, 
            string cacheName = null)
        {
            Repository = repository;
            CacheManager = cacheManager;
            CacheName = cacheName ?? GenerateDefaultCacheName();
            ObjectMapper = NullObjectMapper.Instance;
        }

        public abstract TCacheItem Get(TPrimaryKey id);

        public abstract Task<TCacheItem> GetAsync(TPrimaryKey id);

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

        protected virtual TCacheItem MapToCacheItem(TEntity entity)
        {
            if (ObjectMapper is NullObjectMapper)
            {
                throw new AbpException(
                    string.Format(
                        "MapToCacheItem method should be overridden or IObjectMapper should be implemented in order to map {0} to {1}",
                        typeof (TEntity),
                        typeof (TCacheItem)
                        )
                    );
            }

            return ObjectMapper.Map<TCacheItem>(entity);
        }

        protected virtual string GenerateDefaultCacheName()
        {
            return GetType().FullName;
        }
    }
}
