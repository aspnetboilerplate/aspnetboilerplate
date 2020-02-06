using System;
using System.Threading.Tasks;

namespace Abp.Webhooks
{
    /// <summary>
    /// Null pattern implementation of <see cref="IWebhookSubscriptionsStore"/>.
    /// It's used if <see cref="IWebhookSubscriptionsStore"/> is not implemented by actual persistent store
    /// </summary>
    public class NullWebhookEventStore : IWebhookEventStore
    {
        public static NullWebhookEventStore Instance { get; } = new NullWebhookEventStore();

        public Task<Guid> InsertAndGetIdAsync(WebhookEvent webhookEvent)
        {
            return Task.FromResult<Guid>(default);
        }

        public Guid InsertAndGetId(WebhookEvent webhookEvent)
        {
            return default;
        }

        public Task<WebhookEvent> GetAsync(int? tenantId, Guid id)
        {
            return Task.FromResult<WebhookEvent>(default);
        }

        public WebhookEvent Get(int? tenantId, Guid id)
        {
            return default;
        }
    }
}
