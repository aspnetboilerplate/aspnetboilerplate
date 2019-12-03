using System;

namespace Abp.Runtime.Caching.Configuration
{
    internal class CacheConfigurator : ICacheConfigurator
    {
        public string CacheName { get; private set; }

        public Action<ICacheOptions> InitAction { get; private set; }

        public CacheConfigurator(Action<ICacheOptions> initAction)
        {
            InitAction = initAction;
        }

        public CacheConfigurator(string cacheName, Action<ICacheOptions> initAction)
        {
            CacheName = cacheName;
            InitAction = initAction;
        }
    }
}