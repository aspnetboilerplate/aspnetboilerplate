using System.Collections.Generic;

namespace Abp.RealTime.Redis
{
    public class AbpRedisOnlineClientStore : IOnlineClientStore
    {
        public AbpRedisOnlineClientStore(IAbpRedisOnlineClientStoreOptions options, IAbpRedisOnlineClientStoreDatabaseProvider databaseProvider, IRedisOnlineClientStoreSerializer serializer)
        {
            Store = new RedisKeyValueStore<string, OnlineClient>(options.StoreName, databaseProvider, serializer);
        }

        public void Add(IOnlineClient client)
        {
            Store.TryAdd(client.ConnectionId, client as OnlineClient);
        }

        public bool Remove(string connectionId)
        {
            return TryRemove(connectionId, out IOnlineClient _);
        }

        public bool TryRemove(string connectionId, out IOnlineClient client)
        {
            Store.TryGetValue(connectionId, out OnlineClient found);
          
            bool removed = false;
            
            if (found != null)
                removed = Store.Remove(connectionId);
            client = removed ? found : null;

            return removed;
        }

        public bool TryGet(string connectionId, out IOnlineClient client)
        {
            bool result = Store.TryGetValue(connectionId, out OnlineClient found);
            client = found;
            return result;
        }

        public bool Contains(string connectionId)
        {
            return Store.ContainsKey(connectionId);
        }

        public IReadOnlyList<IOnlineClient> GetAll()
        {
            return Store.Values;
        }

        public void Clear()
        {
            Store.Clear();
        }

        private IRedisKeyValueStore<string, OnlineClient> Store { get; }
    }
}
