using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;
using Abp.Runtime.Caching.Configuration;
using Castle.Core.Logging;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis.InMemory
{
    public class AbpRedisInMemoryCacheManager : CacheManagerBase
    {
        public ILogger Logger { get; set; }

        private readonly AbpRedisCacheOptions _options;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        public AbpRedisInMemoryCacheManager(IIocManager iocManager, ICachingConfiguration configuration) : base(iocManager, configuration)
        {
            Logger = NullLogger.Instance;
            _options = iocManager.Resolve<AbpRedisCacheOptions>();
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);

            this.SetupEvents();
        }

        private ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            return ConnectionMultiplexer.Connect(_options.ConnectionString);
        }

        private void SetupEvents()
        {

            ISubscriber subscriber = _connectionMultiplexer.Value.GetSubscriber();

            subscriber.Subscribe($"__keyspace*__:*", (channel, value) =>
                {
                    var cache = (AbpRedisInMemoryCache)this.GetCache(channel);
                    cache.SetMemory(channel, value);
                }
            );
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return new AbpRedisInMemoryCache(name)
            {
                Logger = Logger
            };
        }

        protected override void DisposeCaches()
        {
            foreach (var cache in Caches.Values)
            {
                cache.Dispose();
            }
        }
    }
}
