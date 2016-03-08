using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using Abp.Dependency;
using Abp.Extensions;

namespace Abp.RedisCache.Configuration
{
    public class AbpRedisConnectionProvider : IAbpRedisConnectionProvider, ISingletonDependency
    {
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> ConnectionMultiplexers = new ConcurrentDictionary<string, ConnectionMultiplexer>();

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

            return ConnectionMultiplexers.GetOrAdd(
                connectionString,
                ConnectionMultiplexer.Connect(connectionString)
                );
        }

        public int GetDatabaseId(string databaseIdAppSettingName)
        {
            var appSetting = ConfigurationManager.AppSettings[databaseIdAppSettingName];
            if (appSetting.IsNullOrEmpty())
            {
                return -1;
            }

            int databaseId;
            if (!int.TryParse(appSetting, out databaseId))
            {
                return -1;
            }

            return databaseId;
        }
    }
}
