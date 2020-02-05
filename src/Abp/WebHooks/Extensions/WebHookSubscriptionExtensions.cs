using System.Linq;

namespace Abp.Webhooks
{
    public static class WebhookSubscriptionExtensions
    {
        /// <summary>
        /// if subscribed to given webhook
        /// </summary>
        /// <returns></returns>
        public static bool IsSubscribed(this WebhookSubscription webhookSubscription, string webhookName)
        {
            if (webhookSubscription.Webhooks == null || webhookSubscription.Webhooks.Count == 0)
            {
                return false;
            }

            return webhookSubscription.Webhooks.Contains(webhookName);
        }

        public static WebhookSubscription ToWebhookSubscription(this WebhookSubscriptionInfo webhookSubscriptionInfo)
        {
            return new WebhookSubscription
            {
                Id = webhookSubscriptionInfo.Id,
                TenantId = webhookSubscriptionInfo.TenantId,
                IsActive = webhookSubscriptionInfo.IsActive,
                Secret = webhookSubscriptionInfo.Secret,
                WebhookUri = webhookSubscriptionInfo.WebhookUri,
                Webhooks = webhookSubscriptionInfo.GetSubscribedWebhooks().ToList(),
                Headers = webhookSubscriptionInfo.GetWebhookHeaders()
            };
        }
    }
}
