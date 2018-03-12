using System;
using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;

namespace Abp.MultiTenancy
{
    /// <summary>
    /// This class handles related events and invalidated tenant feature cache items if needed.
    /// </summary>
    public class TenantFeatureCacheItemInvalidator :
        IEventHandler<EntityChangedEventData<TenantFeatureSetting>>,
        ITransientDependency
    {
        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantFeatureCacheItemInvalidator"/> class.
        /// </summary>
        /// <param name="cacheManager">The cache manager.</param>
        public TenantFeatureCacheItemInvalidator(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityChangedEventData<TenantFeatureSetting> eventData)
        {
            if (!eventData.Entity.TenantId.HasValue)
            {
                throw new Exception("TenantId field of TenantFeatureSetting cannot be null !");
            }

            _cacheManager.GetTenantFeatureCache().Remove(eventData.Entity.TenantId.Value);
        }
    }
}