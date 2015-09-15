using StackExchange.Redis;
using System;

namespace Abp.RedisCache.RedisImpl
{
    public static class RedisDatabaseExtensions
    {
        public static void KeyDeleteWithPrefix(this IDatabase database, string prefix)
        {
            if (database == null)
            {
                throw new ArgumentException("Database cannot be null", "database");
            }

            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException("Prefix cannot be empty", "database");
            }

            database.ScriptEvaluate(@"
                local keys = redis.call('keys', ARGV[1]) 
                for i=1,#keys,5000 do 
                redis.call('del', unpack(keys, i, math.min(i+4999, #keys)))
                end", values: new RedisValue[] { prefix });
        }

        public static int KeyCount(this IDatabase database, string prefix)
        {
            if (database == null)
            {
                throw new ArgumentException("Database cannot be null", "database");
            }

            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException("Prefix cannot be empty", "database");
            }

            var retVal = database.ScriptEvaluate("return table.getn(redis.call('keys', ARGV[1]))", values: new RedisValue[] { prefix });

            if (retVal.IsNull)
            {
                return 0;
            }

            return (int)retVal;
        }
    }
}
