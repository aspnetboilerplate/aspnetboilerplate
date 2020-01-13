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
            if (webhookSubscription.WebhookDefinitions == null || webhookSubscription.WebhookDefinitions.Count == 0)
            {
                return false;
            }

            return webhookSubscription.WebhookDefinitions.Contains(webhookName);
        }

        public static WebhookSubscription ToWebhookSubscription(this WebhookSubscriptionInfo webhookSubscriptionInfo)
        {
            return new WebhookSubscription()
            {
                Id = webhookSubscriptionInfo.Id,
                TenantId = webhookSubscriptionInfo.TenantId,
                IsActive = webhookSubscriptionInfo.IsActive,
                Secret = webhookSubscriptionInfo.Secret,
                WebhookUri = webhookSubscriptionInfo.WebhookUri,
                WebhookDefinitions = webhookSubscriptionInfo.GetSubscribedWebhooks().ToList(),
                Headers = webhookSubscriptionInfo.GetWebhookHeaders()
            };
        }
    }
}
