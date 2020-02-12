using System;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// An interface to work with cache in a typed manner.
    /// Use <see cref="CacheExtensions.AsTyped{TKey,TValue}"/> method
    /// to convert a <see cref="ICache"/> to this interface.
    /// </summary>
    /// <typeparam name="TKey">Key type for cache items</typeparam>
    /// <typeparam name="TValue">Value type for cache items</typeparam>
    public interface ITypedCache<TKey, TValue> : IDisposable, ICacheOptions, IAbpCache<TKey, TValue>
    {
        /// <summary>
        /// Gets the internal cache.
        /// </summary>
        ICache InternalCache { get; }
    }
}
