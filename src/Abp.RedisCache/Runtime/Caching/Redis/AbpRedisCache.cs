using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Reflection.Extensions;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to store cache in a Redis server.
    /// </summary>
    public class AbpRedisCache : CacheBase
    {
        private readonly IDatabase _database;
        private readonly IRedisCacheSerializer _serializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpRedisCache(
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
            var objbyte = _database.StringGet(GetLocalizedRedisKey(key));
            return objbyte.HasValue ? Deserialize(objbyte) : null;
        }

        public override object[] GetOrDefault(string[] keys)
        {
            var redisKeys = keys.Select(GetLocalizedRedisKey);
            var redisValues = _database.StringGet(redisKeys.ToArray());
            var objbytes = redisValues.Select(obj => obj.HasValue ? Deserialize(obj) : null);
            return objbytes.ToArray();
        }

        public override async Task<object> GetOrDefaultAsync(string key)
        {
            var objbyte = await _database.StringGetAsync(GetLocalizedRedisKey(key));
            return objbyte.HasValue ? Deserialize(objbyte) : null;
        }

        public override async Task<object[]> GetOrDefaultAsync(string[] keys)
        {
            var redisKeys = keys.Select(GetLocalizedRedisKey);
            var redisValues = await _database.StringGetAsync(redisKeys.ToArray());
            var objbytes = redisValues.Select(obj => obj.HasValue ? Deserialize(obj) : null);
            return objbytes.ToArray();
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (value == null)
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            _database.StringSet(
                GetLocalizedRedisKey(key),
                Serialize(value, GetSerializableType(value)),
                absoluteExpireTime ?? slidingExpireTime ?? DefaultAbsoluteExpireTime ?? DefaultSlidingExpireTime
                );
        }

        public override async Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (value == null)
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            await _database.StringSetAsync(
                GetLocalizedRedisKey(key),
                Serialize(value, GetSerializableType(value)),
                absoluteExpireTime ?? slidingExpireTime ?? DefaultAbsoluteExpireTime ?? DefaultSlidingExpireTime
                );
        }

        public override void Set(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (pairs.Any(p => p.Value == null))
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            var redisPairs = pairs.Select(p => new KeyValuePair<RedisKey, RedisValue>
                                          (GetLocalizedRedisKey(p.Key), Serialize(p.Value, GetSerializableType(p.Value)))
                                         );

            if (slidingExpireTime.HasValue || absoluteExpireTime.HasValue)
            {
                Logger.WarnFormat("{0}/{1} is not supported for Redis bulk insert of key-value pairs", nameof(slidingExpireTime), nameof(absoluteExpireTime));
            }
            _database.StringSet(redisPairs.ToArray());
        }

        public override async Task SetAsync(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (pairs.Any(p => p.Value == null))
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            var redisPairs = pairs.Select(p => new KeyValuePair<RedisKey, RedisValue>
                                          (GetLocalizedRedisKey(p.Key), Serialize(p.Value, GetSerializableType(p.Value)))
                                         );
            if (slidingExpireTime.HasValue || absoluteExpireTime.HasValue)
            {
                Logger.WarnFormat("{0}/{1} is not supported for Redis bulk insert of key-value pairs", nameof(slidingExpireTime), nameof(absoluteExpireTime));
            }
            await _database.StringSetAsync(redisPairs.ToArray());
        }

        public override void Remove(string key)
        {
            _database.KeyDelete(GetLocalizedRedisKey(key));
        }

        public override async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(GetLocalizedRedisKey(key));
        }

        public override void Remove(string[] keys)
        {
            var redisKeys = keys.Select(GetLocalizedRedisKey);
            _database.KeyDelete(redisKeys.ToArray());
        }

        public override async Task RemoveAsync(string[] keys)
        {
            var redisKeys = keys.Select(GetLocalizedRedisKey);
            await _database.KeyDeleteAsync(redisKeys.ToArray());
        }

        public override void Clear()
        {
            _database.KeyDeleteWithPrefix(GetLocalizedRedisKey("*"));
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

        protected virtual object Deserialize(RedisValue objbyte)
        {
            return _serializer.Deserialize(objbyte);
        }

        protected virtual RedisKey GetLocalizedRedisKey(string key)
        {
            return GetLocalizedKey(key);
        }

        [Obsolete]
        protected virtual string GetLocalizedKey(string key)
        {
            return "n:" + Name + ",c:" + key;
        }
    }
}
