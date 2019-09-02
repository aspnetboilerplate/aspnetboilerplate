using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.RealTime;

namespace Abp.Runtime.Caching.Redis.OnlineClientStore
{
    public class AbpRedisOnlineClientStore : IOnlineClientStore
    {
        protected readonly object SyncObj = new object();

        //TODO: Get this from options
        private const string CacheStoreName = "Abp.Redis.OnlineClientStore.DefaultCacheStoreName";
        private readonly AbpRedisHashCacheManager _cacheManager;

        public AbpRedisOnlineClientStore(AbpRedisHashCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        private ICache GetCache()
        {
            return _cacheManager.GetCache(CacheStoreName);
        }

        public void Add(IOnlineClient client)
        {
            _cacheManager.GetCache(CacheStoreName).Set(client.ConnectionId, client as OnlineClient);
        }

        public bool Remove(string connectionId)
        {
            return TryRemove(connectionId, out IOnlineClient _);
        }

        public bool TryRemove(string connectionId, out IOnlineClient client)
        {
            var cache = GetCache();

            lock (SyncObj)
            {
                cache.Remove(connectionId);
                client = cache.GetOrDefault(connectionId) as IOnlineClient;

                return client == null;
            }
        }

        public bool TryGet(string connectionId, out IOnlineClient client)
        {
            client = GetCache().GetOrDefault(connectionId) as IOnlineClient;
            return client != null;
        }

        public bool Contains(string connectionId)
        {
            return _cacheManager.Contains(CacheStoreName, connectionId);
        }

        public IReadOnlyList<IOnlineClient> GetAll()
        {
            return _cacheManager.GetAllValues(CacheStoreName).Select(item => (IOnlineClient)item).ToImmutableList();
        }

        public void Clear()
        {
            GetCache().Clear();
        }
    }
}
