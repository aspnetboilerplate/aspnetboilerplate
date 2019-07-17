using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    /// store keys and values in redis as hash
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class AbpRedisHashStore<TKey, TValue> : IAbpRedisHashStore<TKey, TValue>
    {
        private readonly IDatabase _database;
        private readonly IRedisCacheSerializer _serializer;
        public string StoreName { get; private set; }

        public AbpRedisHashStore([NotNull]string storeName, IAbpRedisCacheDatabaseProvider databaseProvider, IRedisCacheSerializer serializer)
        {
            StoreName = "HashStore:" + storeName;
            _serializer = serializer;
            _database = databaseProvider.GetDatabase();
        }
        public long Count => _database.HashLength(StoreName);

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                throw new ArgumentException("An item with the same key has already been added");
            }

            Set(key, value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return Set(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _database.HashExists(StoreName, _serializer.Serialize(key, typeof(TKey)));
        }

        public bool Remove(TKey key)
        {
            return _database.HashDelete(StoreName, _serializer.Serialize(key, typeof(TKey)));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);

            var redisValue = _database.HashGet(StoreName, _serializer.Serialize(key, typeof(TKey)));

            if (redisValue.IsNullOrEmpty)
            {
                return false;
            }

            var obj = _serializer.Deserialize(redisValue);

            if (!(obj is TValue returnValue)) return false;

            value = returnValue;

            return true;
        }

        public bool Update(TKey key, TValue value)
        {
            return Set(key, value);
        }

        public void Clear()
        {
            _database.KeyDelete(StoreName);
        }

        private bool Set([NotNull] TKey key, TValue value)
        {
            return _database.HashSet(StoreName, _serializer.Serialize(key, typeof(TKey)), _serializer.Serialize(value, typeof(TValue)));
        }

        public IImmutableList<TKey> GetAllKeys()
        {
            return _database.HashKeys(StoreName).Select(key => (TKey)_serializer.Deserialize(key)).ToImmutableList();
        }


        public IImmutableList<TValue> GetAllValues()
        {
            return _database.HashValues(StoreName).Select(val => (TValue)_serializer.Deserialize(val)).ToImmutableList();
        }

    }
}
