using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Runtime.Caching;

namespace Abp.WebHooks
{
    public class DefaultWebhookStoreManager : IWebhookStoreManager, ITransientDependency
    {
        private readonly ICacheManager _cacheManager;
        public IWebHookStore WebHookStore { get; set; }

        public ITypedCache<Guid, WebHookInfo> InternalCache
        {
            get
            {
                return _cacheManager.GetCache<Guid, WebHookInfo>(GetCacheKey());
            }
        }

        protected virtual string GetCacheKey()
        {   
            return "WebHookInfoByIdCache";
        }

        public DefaultWebhookStoreManager(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            WebHookStore = NullWebHookStore.Instance;
        }

        public Task<Guid> InsertAndGetIdAsync(WebHookInfo webHookInfo)
        {
            return WebHookStore.InsertAndGetIdAsync(webHookInfo);
        }

        public Guid InsertAndGetId(WebHookInfo webHookInfo)
        {
            return WebHookStore.InsertAndGetId(webHookInfo);
        }

        public Task<WebHookInfo> GetAsync(Guid id)
        {
            return InternalCache.GetAsync(id, guid => WebHookStore.GetAsync(guid));
        }

        public WebHookInfo Get(Guid id)
        {
            return InternalCache.Get(id, guid => WebHookStore.Get(guid));
        }


    }
}
