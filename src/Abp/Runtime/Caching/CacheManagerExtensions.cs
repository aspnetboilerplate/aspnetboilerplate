namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Extension methods for <see cref="ICacheManager"/>.
    /// </summary>
    public static class CacheManagerExtensions
    {
        public static ITypedCache<TKey, TValue> GetCache<TKey, TValue>(this ICacheManager cacheManager, string name)
        {
            return cacheManager.GetCache(name).AsTyped<TKey, TValue>();
        }
    }
}