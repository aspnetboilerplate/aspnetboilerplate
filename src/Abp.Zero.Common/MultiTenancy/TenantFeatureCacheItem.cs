using System;
using System.Collections.Generic;

namespace Abp.MultiTenancy
{
    /// <summary>
    /// Used to store features of a Tenant in the cache.
    /// </summary>
    [Serializable]
    public class TenantFeatureCacheItem
    {
        /// <summary>
        /// The cache store name.
        /// </summary>
        public const string CacheStoreName = "AbpZeroTenantFeatures";

        /// <summary>
        /// Edition of the tenant.
        /// </summary>
        public int? EditionId { get; set; }

        /// <summary>
        /// Feature values.
        /// </summary>
        public IDictionary<string, string> FeatureValues { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantFeatureCacheItem"/> class.
        /// </summary>
        public TenantFeatureCacheItem()
        {
            FeatureValues = new Dictionary<string, string>();
        }
    }
}