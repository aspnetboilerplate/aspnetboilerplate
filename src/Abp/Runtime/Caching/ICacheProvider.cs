using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// An upper level container for <see cref="ICacheStore{TKey,TValue}"/> objects. 
    /// A caching provider should work as Singleton and track and manage <see cref="ICacheStore{TKey,TValue}"/> objects.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Gets all cache stores.
        /// </summary>
        /// <returns>List of cache stores</returns>
        IReadOnlyList<ICacheStoreCommon> GetAllCacheStores();

        /// <summary>
        /// Gets (or creates) a cache store.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys of the cache store</typeparam>
        /// <typeparam name="TValue">Type of the values of the cache store</typeparam>
        /// <param name="name">Unique name of the cache store</param>
        /// <param name="defaultSlidingExpireTime">Default cache expire time (sliding)</param>
        /// <returns>The cache store reference</returns>
        ICacheStore<TKey, TValue> GetOrCreateCacheStore<TKey, TValue>(string name, TimeSpan? defaultSlidingExpireTime = null);

        /// <summary>
        /// Gets (or creates) a cache store.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys of the cache store</typeparam>
        /// <typeparam name="TValue">Type of the values of the cache store</typeparam>
        /// <param name="name">Unique name of the cache store</param>
        /// <param name="defaultSlidingExpireTime">Default cache expire time (sliding)</param>
        /// <returns>The cache store reference</returns>
        Task<ICacheStore<TKey, TValue>> GetOrCreateCacheStoreAsync<TKey, TValue>(string name, TimeSpan? defaultSlidingExpireTime = null);

        /// <summary>
        /// Completely removes and disposes a <see cref="ICacheStore{TKey,TValue}"/> based on it's unique name.
        /// Does nothing if there is not a <see cref="ICacheStore{TKey,TValue}"/> with the given <see cref="name"/>.
        /// </summary>
        /// <param name="name">Unique name of the cache store</param>
        /// <returns>True, if there was a cache store with the <see cref="name"/></returns>
        bool RemoveCacheStore(string name);

        /// <summary>
        /// Completely removes and disposes a <see cref="ICacheStore{TKey,TValue}"/> based on it's unique name.
        /// Does nothing if there is not a <see cref="ICacheStore{TKey,TValue}"/> with the given <see cref="name"/>.
        /// </summary>
        /// <param name="name">Unique name of the cache store</param>
        /// <returns>True, if there was a cache store with the <see cref="name"/></returns>
        Task<bool> RemoveCacheStoreAsync(string name);

        /// <summary>
        /// Completely removes and disposes all <see cref="ICacheStore{TKey,TValue}"/> objects.
        /// </summary>
        void RemoveAllCacheStores();

        /// <summary>
        /// Completely removes and disposes all <see cref="ICacheStore{TKey,TValue}"/> objects.
        /// </summary>
        Task RemoveAllCacheStoresAsync();
    }
}
