using System;

namespace Abp.Runtime.Caching.Configuration
{
    internal class CacheConfigurator : ICacheConfigurator
    {
        public CacheConfigurator(Action<ICache> initAction)
        {
            InitAction = initAction;
        }

        public CacheConfigurator(string cacheName, Action<ICache> initAction)
        {
            CacheName = cacheName;
            InitAction = initAction;
        }

        public string CacheName { get; }

        public Action<ICache> InitAction { get; }
    }
}