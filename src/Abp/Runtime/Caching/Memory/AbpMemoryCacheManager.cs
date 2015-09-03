using System.Runtime.Caching;
using Abp.Dependency;

namespace Abp.Runtime.Caching.Memory
{
    /// <summary>
    /// Implements <see cref="ICacheManager"/> to work with <see cref="MemoryCache"/>.
    /// </summary>
    public class AbpMemoryCacheManager : CacheManagerBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="iocManager"></param>
        public AbpMemoryCacheManager(IIocManager iocManager)
            : base(iocManager)
        {
            IocManager.RegisterIfNot<AbpMemoryCache>(DependencyLifeStyle.Transient);
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return IocManager.Resolve<AbpMemoryCache>(new { name });
        }
    }
}
