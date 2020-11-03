namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Extension methods for <see cref="ICache"/>.
    /// </summary>
    public static class CacheExtensions
    {
        public static ITypedCache<TKey, TValue> AsTyped<TKey, TValue>(this ICache cache)
        {
            return new TypedCacheWrapper<TKey, TValue>(cache);
        }
    }
}
