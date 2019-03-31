using System;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using StackExchange.Redis;

namespace Abp.RealTime.Redis
{
    public class RedisKeyValueStore<TKey, TValue> : IRedisKeyValueStore<TKey, TValue>
    {
        private readonly Lazy<IDatabase> _database;
        private readonly IRedisOnlineClientStoreSerializer _serializer;
        private readonly string _storeName;

        public RedisKeyValueStore([NotNull] string storeName, IAbpRedisOnlineClientStoreDatabaseProvider databaseProvider, IRedisOnlineClientStoreSerializer serializer)
        {
            _storeName = storeName;
            _serializer = serializer;
            _database = new Lazy<IDatabase>(databaseProvider.GetDatabase);
        }

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new ArgumentException("An item with the same key has already been added");

            Set(key, value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return Set(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _database.Value.HashExists(_storeName, _serializer.Serialize(key));
        }

        public bool Remove(TKey key)
        {
            return _database.Value.HashDelete(_storeName, _serializer.Serialize(key));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            
            var redisValue = _database.Value.HashGet(_storeName, _serializer.Serialize(key));
            
            if (redisValue.IsNullOrEmpty)
                return false;
            
            value = _serializer.Deserialize<TValue>(redisValue);

            return true;
        }

        public bool Update(TKey key, TValue value)
        {
           return Set(key, value);
        }

        public void Clear()
        {
            _database.Value.KeyDelete(_storeName);
        }

        public int Count => (int) _database.Value.HashLength(_storeName);

        private bool Set([NotNull] TKey key, TValue value)
        {
            return _database.Value.HashSet(_storeName, _serializer.Serialize(key), _serializer.Serialize(value));
        }

        public IImmutableList<TKey> Keys
        {
            get { return _database.Value.HashKeys(_storeName).Select(key => _serializer.Deserialize<TKey>(key)).ToImmutableList(); }
        }

        public IImmutableList<TValue> Values
        {
            get
            {
               return _database.Value.HashValues(_storeName).Select(val => _serializer.Deserialize<TValue>(val)).ToImmutableList();
            }
        }
     
    }
}