using System;
using System.Threading.Tasks;

namespace Abp.Webhooks
{
    /// <summary>
    /// Null pattern implementation of <see cref="IWebhookSubscriptionsStore"/>.
    /// It's used if <see cref="IWebhookSubscriptionsStore"/> is not implemented by actual persistent store
    /// </summary>
    public class NullWebhookStore : IWebhookStore
    {
        public static NullWebhookStore Instance { get; } = new NullWebhookStore();

        public Task<Guid> InsertAndGetIdAsync(WebhookInfo webhookInfo)
        {
            return Task.FromResult<Guid>(default);
        }

        public Guid InsertAndGetId(WebhookInfo webhookInfo)
        {
            return default;
        }

        public Task<WebhookInfo> GetAsync(int? tenantId, Guid id)
        {
            return Task.FromResult<WebhookInfo>(default);
        }

        public WebhookInfo Get(int? tenantId, Guid id)
        {
            return default;
        }
    }
}
