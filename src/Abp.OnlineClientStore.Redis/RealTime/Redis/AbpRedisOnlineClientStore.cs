using System;
using System.Collections.Generic;
using Abp.Data;

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

        public ConditionalValue<IOnlineClient> Remove(string connectionId)
        {
            Store.TryGetValue(connectionId, out OnlineClient value);

            if (value != null)
            {
               bool removed = Store.Remove(connectionId);
               return new ConditionalValue<IOnlineClient>(removed, value);
            }

            return new ConditionalValue<IOnlineClient>(false, null);
        }

        public ConditionalValue<IOnlineClient> Get(string connectionId)
        {
            return Store.TryGetValue(connectionId, out OnlineClient found) ? new ConditionalValue<IOnlineClient>(true, found) : new ConditionalValue<IOnlineClient>(false, null);
        }

        public bool Contains(string connectionId)
        {
            return Store.ContainsKey(connectionId);
        }

        public IReadOnlyList<IOnlineClient> GetAll()
        {
            return Store.Values;
        }

        private IRedisKeyValueStore<string, OnlineClient> Store { get; }
    }
}
