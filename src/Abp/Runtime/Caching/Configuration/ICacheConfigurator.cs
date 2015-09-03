using System;

namespace Abp.Runtime.Caching.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheConfigurator
    {
        string CacheName { get; }

        Action<ICache> InitAction { get; }
    }
}