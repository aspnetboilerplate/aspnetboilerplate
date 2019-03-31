using StackExchange.Redis;

namespace Abp.RealTime.Redis
{
    public interface IRedisOnlineClientStoreSerializer
    {
        string Serialize(object source);

        T Deserialize<T>(RedisValue redisValue);
    }
}