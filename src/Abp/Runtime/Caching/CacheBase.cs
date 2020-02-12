namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Base class for caches.
    /// It's used to simplify implementing <see cref="ICache"/>.
    /// </summary>
    public abstract class CacheBase : AbpCacheBase<string, object>, ICache
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        protected CacheBase(string name) : base(name)
        {
        }
    }
}
