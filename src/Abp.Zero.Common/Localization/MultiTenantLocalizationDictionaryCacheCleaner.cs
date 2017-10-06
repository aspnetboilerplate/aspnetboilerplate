using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;

namespace Abp.Localization
{
    /// <summary>
    /// Clears related localization cache when a <see cref="ApplicationLanguageText"/> changes.
    /// </summary>
    public class MultiTenantLocalizationDictionaryCacheCleaner : 
        ITransientDependency,
        IEventHandler<EntityChangedEventData<ApplicationLanguageText>>
    {
        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTenantLocalizationDictionaryCacheCleaner"/> class.
        /// </summary>
        public MultiTenantLocalizationDictionaryCacheCleaner(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityChangedEventData<ApplicationLanguageText> eventData)
        {
            _cacheManager
                .GetMultiTenantLocalizationDictionaryCache()
                .Remove(MultiTenantLocalizationDictionaryCacheHelper.CalculateCacheKey(
                    eventData.Entity.TenantId,
                    eventData.Entity.Source,
                    eventData.Entity.LanguageName)
                );
        }
    }
}