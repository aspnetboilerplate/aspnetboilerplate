using System;

namespace Abp.Runtime.Caching
{
    public interface ICacheOptions
    {
        /// <summary>
        /// Default sliding expire time of cache items.
        /// Default value: 60 minutes (1 hour).
        /// Can be changed by configuration.
        /// </summary>
        TimeSpan DefaultSlidingExpireTime { get; set; }

        /// <summary>
        /// Default absolute expire time factory of cache items.
        /// Priority is higher than DefaultAbsoluteExpireTime
        /// Default value: null (not used).
        /// </summary>
        Func<string, DateTimeOffset> DefaultAbsoluteExpireTimeFactory { get; set; }

        /// <summary>
        /// Default absolute expire time of cache items.
        /// Default value: null (not used).
        /// </summary>
        DateTimeOffset? DefaultAbsoluteExpireTime { get; set; }
    }
}
