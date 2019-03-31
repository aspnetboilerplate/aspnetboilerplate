using System;
using Abp.Dependency;
using StackExchange.Redis;

namespace Abp.RealTime.Redis
{
    public class AbpRedisOnlineClientStoreDatabaseProvider : IAbpRedisOnlineClientStoreDatabaseProvider, ISingletonDependency
    {
        private readonly IAbpRedisOnlineClientStoreOptions _options;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpRedisOnlineClientStoreDatabaseProvider"/> class.
        /// </summary>
        public AbpRedisOnlineClientStoreDatabaseProvider(IAbpRedisOnlineClientStoreOptions options)
        {
            _options = options;
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.Value.GetDatabase();
        }

        private ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            var configuration = ConfigurationOptions.Parse(_options.ConnectionString);
            configuration.DefaultDatabase = _options.DatabaseId;

            return ConnectionMultiplexer.Connect(configuration);
        }
    }
}