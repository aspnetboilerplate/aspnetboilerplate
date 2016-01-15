using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Configuration;

namespace Abp.RedisCache.Configuration
{
    public class AbpRedisConnectionProvider : IAbpRedisConnectionProvider
    {
        private static readonly ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>> ConnectionMultiplexers = new ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>>();

        public string GetConnectionString(string name)
        {
            var connStr = ConfigurationManager.ConnectionStrings[name];
            if (connStr == null)
            {
                return "localhost";
            }

            return connStr.ConnectionString;
        }

        public ConnectionMultiplexer GetConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            // when using ConcurrentDictionary, multiple threads can create the value
            // at the same time, so we need to pass a Lazy so that it's only 
            // the object which is added that will create a ConnectionMultiplexer,
            // even when a delegate is passed

            return ConnectionMultiplexers.GetOrAdd(
                connectionString,
                new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString))
                ).Value;
        }
    }
}
