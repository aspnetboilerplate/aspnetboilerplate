using System;
using System.Collections.Generic;

namespace Abp.Runtime.Caching.Configuration
{
    /// <summary>
    /// Used to configure caching system.
    /// </summary>
    public interface ICachingConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        IReadOnlyList<ICacheConfigurator> Configurators { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initAction"></param>
        void ConfigureAll(Action<ICache> initAction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="initAction"></param>
        void Configure(string cacheName, Action<ICache> initAction);
    }
}
