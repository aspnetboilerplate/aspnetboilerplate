using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Abp.RealTime;

namespace Abp.Runtime.Caching.Redis.OnlineClientStore
{
    public class AbpRedisOnlineClientStore : IOnlineClientStore
    {
        //TODO: Get this from options
        private const string CacheStoreName = "Abp.Redis.OnlineClientStore.DefaultCacheStoreName";
        private readonly IAbpRedisHashStore<string, OnlineClient> _redisHashStore;

        public AbpRedisOnlineClientStore(IAbpRedisHashStoreProvider abpRedisHashStoreProvider)
        {
            _redisHashStore = abpRedisHashStoreProvider.GetAbpRedisHashStore<string, OnlineClient>(CacheStoreName);
        }
        public void Add(IOnlineClient client)
        {
            _redisHashStore.TryAdd(client.ConnectionId, client as OnlineClient);
        }

        public bool Remove(string connectionId)
        {
            return TryRemove(connectionId, out IOnlineClient _);
        }

        public bool TryRemove(string connectionId, out IOnlineClient client)
        {
            _redisHashStore.TryGetValue(connectionId, out OnlineClient found);

            bool removed = false;

            if (found != null)
                removed = _redisHashStore.Remove(connectionId);
            client = removed ? found : null;

            return removed;
        }

        public bool TryGet(string connectionId, out IOnlineClient client)
        {
            bool result = _redisHashStore.TryGetValue(connectionId, out OnlineClient found);
            client = found;
            return result;
        }

        public bool Contains(string connectionId)
        {
            return _redisHashStore.ContainsKey(connectionId);
        }

        public IReadOnlyList<IOnlineClient> GetAll()
        {
            return _redisHashStore.GetAllValues();
        }

        public void Clear()
        {
            _redisHashStore.Clear();
        }
    }
}
