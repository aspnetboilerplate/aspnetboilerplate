using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Reflection.Extensions;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis
{
    public class AbpRedisHashCache : CacheBase
    {
        private readonly IDatabase _database;
        private readonly IRedisCacheSerializer _serializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpRedisHashCache(
            string name,
            IAbpRedisCacheDatabaseProvider redisCacheDatabaseProvider,
            IRedisCacheSerializer redisCacheSerializer)
            : base(name)
        {
            _database = redisCacheDatabaseProvider.GetDatabase();
            _serializer = redisCacheSerializer;
        }

        public override object GetOrDefault(string key)
        {
            var redisValue = _database.HashGet(Name, GetHashField(key));

            if (redisValue.IsNullOrEmpty)
            {
                return null;
            }

            return Deserialize(redisValue);
        }

        private RedisValue GetHashField(string key)
        {
            return _serializer.Serialize(key, typeof(RedisValue));
        }

        public override object[] GetOrDefault(string[] keys)
        {
            var redisKeys = keys.Select(GetHashField).ToArray();
            var redisValues = _database.HashGet(Name, redisKeys);

            var objBytes = redisValues.Select(obj => obj.HasValue ? Deserialize(obj) : null);
            return objBytes.ToArray();
        }

        public override async Task<object> GetOrDefaultAsync(string key)
        {
            var redisValue = await _database.HashGetAsync(Name, GetHashField(key));

            if (redisValue.IsNullOrEmpty)
            {
                return null;
            }

            return Deserialize(redisValue);
        }

        public override async Task<object[]> GetOrDefaultAsync(string[] keys)
        {
            var redisKeys = keys.Select(GetHashField).ToArray();
            var redisValues = await _database.HashGetAsync(Name, redisKeys);

            var objBytes = redisValues.Select(obj => obj.HasValue ? Deserialize(obj) : null);
            return objBytes.ToArray();
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (value == null)
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            _database.HashSet(Name, GetHashField(key), Serialize(value, GetSerializableType(value)));
        }

        public override async Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (value == null)
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            await _database.HashSetAsync(Name, GetHashField(key), Serialize(value, GetSerializableType(value)));
        }

        public override void Set(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (pairs.Any(p => p.Value == null))
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            var redisPairs = pairs.Select(p => new HashEntry(GetHashField(p.Key), Serialize(p.Value, GetSerializableType(p.Value))));

            if (slidingExpireTime.HasValue || absoluteExpireTime.HasValue)
            {
                Logger.WarnFormat("{0}/{1} is not supported for Redis bulk insert of key-value pairs", nameof(slidingExpireTime), nameof(absoluteExpireTime));
            }

            _database.HashSet(Name, redisPairs.ToArray());
        }

        public override async Task SetAsync(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (pairs.Any(p => p.Value == null))
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            var redisPairs = pairs.Select(p => new HashEntry(GetHashField(p.Key), Serialize(p.Value, GetSerializableType(p.Value))));

            if (slidingExpireTime.HasValue || absoluteExpireTime.HasValue)
            {
                Logger.WarnFormat("{0}/{1} is not supported for Redis bulk insert of key-value pairs", nameof(slidingExpireTime), nameof(absoluteExpireTime));
            }

            await _database.HashSetAsync(Name, redisPairs.ToArray());
        }

        public override void Remove(string key)
        {
            _database.HashDelete(Name, GetHashField(key));
        }

        public override async Task RemoveAsync(string key)
        {
            await _database.HashDeleteAsync(Name, GetHashField(key));
        }

        public override void Remove(string[] keys)
        {
            var redisKeys = keys.Select(GetHashField).ToArray();
            _database.HashDelete(Name, redisKeys);
        }

        public override async Task RemoveAsync(string[] keys)
        {
            var redisKeys = keys.Select(GetHashField).ToArray();
            await _database.HashDeleteAsync(Name, redisKeys);
        }

        public override void Clear()
        {
            _database.KeyDelete(Name);
        }

        public IImmutableList<string> GetAllKeys()
        {
            return _database.HashKeys(Name).Select(key => Deserialize(key).ToString()).ToImmutableList();
        }

        public IImmutableList<object> GetAllValues()
        {
            return _database.HashValues(Name).Select(Deserialize).ToImmutableList();
        }

        public bool Contains(string key)
        {
            return _database.HashExists(Name, GetHashField(key));
        }

        protected virtual Type GetSerializableType(object value)
        {
            //TODO: This is a workaround for serialization problems of entities.
            //TODO: Normally, entities should not be stored in the cache, but currently Abp.Zero packages does it. It will be fixed in the future.
            var type = value.GetType();
            if (EntityHelper.IsEntity(type) && type.GetAssembly().FullName.Contains("EntityFrameworkDynamicProxies"))
            {
                type = type.GetTypeInfo().BaseType;
            }

            return type;
        }

        protected virtual string Serialize(object value, Type type)
        {
            return _serializer.Serialize(value, type);
        }

        protected virtual object Deserialize(RedisValue objByte)
        {
            return _serializer.Deserialize(objByte);
        }
    }
}