using Abp.Runtime.Caching;
using StackExchange.Redis;
using System;
using Abp.RedisCache.Configuration;
using Abp.RedisCache.RedisImpl;
using Abp.Runtime.Serialization;

namespace Abp.RedisCache
{
    public class AbpRedisCache : CacheBase
    {

        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly AbpRedisCacheConfig _config;

        public IDatabase Database
        {
            get
            {
                return _connectionMultiplexer.GetDatabase(DatabaseId);
            }
        }

        public int DatabaseId { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpRedisCache(string name, IAbpRedisConnectionProvider redisConnectionProvider, AbpRedisCacheConfig config)
            : base(name)
        {
            _config = config;
            var connectionString = redisConnectionProvider.GetConnectionString(_config.ConnectionStringKey);
            DatabaseId = redisConnectionProvider.GetDatabaseId(_config.DatabaseIdAppSetting);
            _connectionMultiplexer = redisConnectionProvider.GetConnection(connectionString);
        }
        public override object GetOrDefault(string key)
        {
            var objbyte = Database.StringGet(GetLocalizedKey(key));
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

            Database.StringSet(
                GetLocalizedKey(key),
                BinarySerializationHelper.Serialize(value),
                slidingExpireTime
                );
        }

        public override void Remove(string key)
        {
            Database.KeyDelete(GetLocalizedKey(key));
        }

        public override void Clear()
        {
            Database.KeyDeleteWithPrefix(GetLocalizedKey("*"));
        }

        private string GetLocalizedKey(string key)
        {
            return "n:" + Name + ",c:" + key;
        }
    }
}
