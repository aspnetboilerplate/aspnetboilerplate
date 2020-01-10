using System;
using System.Threading.Tasks;

namespace Abp.WebHooks
{
    /// <summary>
    /// Null pattern implementation of <see cref="IWebHookSubscriptionsStore"/>.
    /// It's used if <see cref="IWebHookSubscriptionsStore"/> is not implemented by actual persistent store
    /// </summary>
    public class NullWebHookStore : IWebHookStore
    {
        public static NullWebHookStore Instance { get; } = new NullWebHookStore();

        public Task<Guid> InsertAndGetIdAsync(WebHookInfo webHookInfo)
        {
            return Task.FromResult<Guid>(default);
        }

        public Guid InsertAndGetId(WebHookInfo webHookInfo)
        {
            return default;
        }

        public Task<WebHookInfo> GetAsync(int? tenantId, Guid id)
        {
            return Task.FromResult<WebHookInfo>(default);
        }

        public WebHookInfo Get(int? tenantId, Guid id)
        {
            return default;
        }
    }
}
