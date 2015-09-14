using Abp.RedisCache.Configuration;
using Abp.Runtime.Caching;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.RedisCache.RedisImpl;
namespace Abp.RedisCache
{
    public class AbpRedisCache : CacheBase
    {
        public const string ConnectionStringKey = "Abp.Redis.Cache";
        private readonly IAbpRedisConnectionProvider _redisConnectionProvider;
        private readonly string _connectionString;
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public IDatabase Database
        {
            get
            {
                return _connectionMultiplexer.GetDatabase();
            }
        }
       
        private string cachename;
        private IDatabase database;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Unique name of the cache</param>
        public AbpRedisCache(string name, IAbpRedisConnectionProvider redisConnectionProvider)
            : base(name)
        {
            cachename = name;
            _redisConnectionProvider = redisConnectionProvider;
            _connectionString = _redisConnectionProvider.GetConnectionString(ConnectionStringKey);
            _connectionMultiplexer = _redisConnectionProvider.GetConnection(_connectionString);
        }
        public override object GetOrDefault(string key)
        {

            var objbyte = Database.StringGet(GetLocalizedKey(key));
            if (!objbyte.HasValue)
            {
                return null;
            }
            else
            {
                return SerializeUtil.Deserialize(objbyte);
            }
           
            
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
            return "n:"+cachename + ",c:" + key;
        }
    }
}
