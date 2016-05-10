using System;
using System.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    ///     Implements <see cref="IAbpRedisCacheDatabaseProvider" />.
    /// </summary>
    public class AbpRedisCacheDatabaseProvider : IAbpRedisCacheDatabaseProvider, ISingletonDependency
    {
        private const string ConnectionStringKey = "Abp.Redis.Cache";
        private const string DatabaseIdSettingKey = "Abp.Redis.Cache.DatabaseId";

        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AbpRedisCacheDatabaseProvider" /> class.
        /// </summary>
        public AbpRedisCacheDatabaseProvider()
        {
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
        }

        /// <summary>
        ///     Gets the database connection.
        /// </summary>
        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.Value.GetDatabase(GetDatabaseId());
        }

        private static ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            return ConnectionMultiplexer.Connect(GetConnectionString());
        }

        private static int GetDatabaseId()
        {
            var appSetting = ConfigurationManager.AppSettings[DatabaseIdSettingKey];
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

        private static string GetConnectionString()
        {
            var connStr = ConfigurationManager.ConnectionStrings[ConnectionStringKey];
            if (connStr == null || connStr.ConnectionString.IsNullOrWhiteSpace())
            {
                return "localhost";
            }

            return connStr.ConnectionString;
        }
    }
}