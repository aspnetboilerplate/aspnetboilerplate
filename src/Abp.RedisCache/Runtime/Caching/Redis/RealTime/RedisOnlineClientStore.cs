using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Dependency;
using Abp.RealTime;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis.RealTime
{
    public class RedisOnlineClientStore<T> : RedisOnlineClientStore, IOnlineClientStore<T>
    {
        public RedisOnlineClientStore(
            IAbpRedisCacheDatabaseProvider database,
            AbpRedisCacheOptions options) : base(database, options)
        {
        }
    }

    public class RedisOnlineClientStore : IOnlineClientStore, ISingletonDependency
    {
        private readonly IAbpRedisCacheDatabaseProvider _database;

        private readonly string _clientStoreKey;

        public RedisOnlineClientStore(
            IAbpRedisCacheDatabaseProvider database,
            AbpRedisCacheOptions options)
        {
            _database = database;

            _clientStoreKey = options.OnlineClientsStoreKey + ".Clients";
        }

        public void Add(IOnlineClient client)
        {
            var database = GetDatabase();
            database.HashSet(_clientStoreKey, new[]
            {
                new HashEntry(client.ConnectionId, client.ToString())
            });
        }

        public bool Remove(string connectionId)
        {
            var database = GetDatabase();

            var clientValue = database.HashGet(_clientStoreKey, connectionId);
            if (clientValue.IsNullOrEmpty)
            {
                return true;
            }
            
            database.HashDelete(_clientStoreKey, connectionId);
            return true;
        }

        public bool TryRemove(string connectionId, out IOnlineClient client)
        {
            try
            {
                var database = GetDatabase();

                var clientValue = database.HashGet(_clientStoreKey, connectionId);
                if (clientValue.IsNullOrEmpty)
                {
                    client = null;
                    return true;
                }

                client = JsonConvert.DeserializeObject<OnlineClient>(clientValue);
            
                database.HashDelete(_clientStoreKey, connectionId);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                client = null;
                return false;
            }
        }

        public bool TryGet(string connectionId, out IOnlineClient client)
        {
            var database = GetDatabase();
            var clientValue = database.HashGet(_clientStoreKey, connectionId);
            if (clientValue.IsNullOrEmpty)
            {
                client = null;
                return false;
            }

            client = JsonConvert.DeserializeObject<OnlineClient>(clientValue);
            return true;
        }

        public bool Contains(string connectionId)
        {
            var database = GetDatabase();
            var clientValue = database.HashGet(_clientStoreKey, connectionId);
            return !clientValue.IsNullOrEmpty;
        }

        public IReadOnlyList<IOnlineClient> GetAll()
        {
            var database = GetDatabase();
            var clientsEntries = database.HashGetAll(_clientStoreKey);
            var clients = new List<IOnlineClient>();
            foreach (var entry in clientsEntries)
            {
                clients.Add(JsonConvert.DeserializeObject<OnlineClient>(entry.Value));
            }

            return clients.ToImmutableList();
        }

        private IDatabase GetDatabase()
        {
            return _database.GetDatabase();
        }
    }
}