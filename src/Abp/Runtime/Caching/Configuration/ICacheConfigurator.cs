using System;

namespace Abp.Runtime.Caching.Configuration
{
    /// <summary>
    /// A registered cache configurator.
    /// </summary>
    public interface ICacheConfigurator
    {
        /// <summary>
        /// Name of the cache.
        /// It will be null if this configurator configures all caches.
        /// </summary>
        string CacheName { get; }

        /// <summary>
        /// Configuration action. Called just after the cache is created.
        /// </summary>
        Action<ICacheOptions> InitAction { get; }
    }
}