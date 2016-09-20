using System;
using Abp.Domain.Entities;
using Abp.Json;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to store cache in a Redis server.
    /// </summary>
    public class AbpRedisCache : CacheBase
    {
        private readonly IDatabase _database;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpRedisCache(string name, IAbpRedisCacheDatabaseProvider redisCacheDatabaseProvider)
            : base(name)
        {
            _database = redisCacheDatabaseProvider.GetDatabase();
        }

        public override object GetOrDefault(string key)
        {
            var objbyte = _database.StringGet(GetLocalizedKey(key));
            return objbyte.HasValue
                ? JsonSerializationHelper.DeserializeWithType(objbyte)
                : null;
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (value == null)
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            //TODO: This is a workaround for serialization problems of entities.
            //TODO: Normally, entities should not be stored in the cache, but currently Abp.Zero packages does it. It will be fixed in the future.
            var type = value.GetType();
            if (EntityHelper.IsEntity(type) && type.Assembly.FullName.Contains("EntityFrameworkDynamicProxies"))
            {
                type = type.BaseType;
            }

            _database.StringSet(
                GetLocalizedKey(key),
                JsonSerializationHelper.SerializeWithType(value, type),
                absoluteExpireTime ?? slidingExpireTime ?? DefaultAbsoluteExpireTime ?? DefaultSlidingExpireTime
                );
        }

        public override void Remove(string key)
        {
            _database.KeyDelete(GetLocalizedKey(key));
        }

        public override void Clear()
        {
            _database.KeyDeleteWithPrefix(GetLocalizedKey("*"));
        }

        private string GetLocalizedKey(string key)
        {
            return "n:" + Name + ",c:" + key;
        }
    }
}
