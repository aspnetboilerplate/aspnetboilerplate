using Abp.Dependency;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Abp.RealTime.Redis
{
    /// <summary>
    ///     Default implementation uses JSON as the underlying persistence mechanism.
    /// </summary>
    public class DefaultRedisOnlineClientStoreSerializer : IRedisOnlineClientStoreSerializer, ITransientDependency
    {
        public T Deserialize<T>(RedisValue redisValue)
        {
            return JsonConvert.DeserializeObject<T>(redisValue);
        }

        public string Serialize(object source)
        {
             return JsonConvert.SerializeObject(source);
        }
    }
}