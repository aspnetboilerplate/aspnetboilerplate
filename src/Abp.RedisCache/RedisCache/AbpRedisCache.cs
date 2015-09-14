using Abp.Runtime.Caching;
using StackExchange.Redis;
using System;
using Abp.RedisCache.Configuration;
using Abp.RedisCache.RedisImpl;

namespace Abp.RedisCache
{
    public class AbpRedisCache : CacheBase
    {
        public const string ConnectionStringKey = "Abp.Redis.Cache";

        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public IDatabase Database
        {
            get
            {
                return _connectionMultiplexer.GetDatabase();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpRedisCache(string name, IAbpRedisConnectionProvider redisConnectionProvider)
            : base(name)
        {
            var connectionString = redisConnectionProvider.GetConnectionString(ConnectionStringKey);
            _connectionMultiplexer = redisConnectionProvider.GetConnection(connectionString);
        }
        public override object GetOrDefault(string key)
        {
            var objbyte = Database.StringGet(GetLocalizedKey(key));
            return objbyte.HasValue
                ? SerializeUtil.Deserialize(objbyte)
                : null;
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null)
        {
            var obj = SerializeUtil.Serialize(value);
            Database.StringSet(GetLocalizedKey(key), obj, slidingExpireTime);
        }

        public override void Remove(string key)
        {
            Database.KeyDelete(key);
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
