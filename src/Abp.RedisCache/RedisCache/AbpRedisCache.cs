using Abp.Runtime.Caching;
using StackExchange.Redis;
using System;
using Abp.Runtime.Serialization;

namespace Abp.RedisCache
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
                ? BinarySerializationHelper.DeserializeExtended(objbyte)
                : null;
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null)
        {
            if (value == null)
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            _database.StringSet(
                GetLocalizedKey(key),
                BinarySerializationHelper.Serialize(value),
                slidingExpireTime
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
