using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Webhooks
{
    public interface IWebhookSubscriptionManager
    {
        /// <summary>
        /// Returns subscription for given id. 
        /// </summary>
        /// <param name="id">Unique identifier of <see cref="WebhookSubscriptionInfo"/></param>
        Task<WebhookSubscription> GetAsync(Guid id);

        /// <summary>
        /// Returns subscription for given id. 
        /// </summary>
        /// <param name="id">Unique identifier of <see cref="WebhookSubscriptionInfo"/></param>
        WebhookSubscription Get(Guid id);

        /// <summary>
        /// Returns all subscriptions of tenant
        /// </summary>
        /// <returns></returns>
        Task<List<WebhookSubscription>> GetAllSubscriptionsAsync(int? tenantId);

        /// <summary>
        /// Returns all subscriptions of tenant
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        List<WebhookSubscription> GetAllSubscriptions(int? tenantId);

        /// <summary>
        /// Returns all subscriptions for given webhook.
        /// </summary>
        /// <param name="webhookName"><see cref="WebhookDefinition.Name"/></param>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        Task<List<WebhookSubscription>> GetAllSubscriptionsIfFeaturesGrantedAsync(int? tenantId, string webhookName);

        /// <summary>
        /// Returns all subscriptions for given webhook.
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        /// <param name="webhookName"><see cref="WebhookDefinition.Name"/></param>
        List<WebhookSubscription> GetAllSubscriptionsIfFeaturesGranted(int? tenantId, string webhookName);

        /// <summary>
        /// Checks if tenant subscribed for a webhook. (Checks if webhook features are granted)
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        /// <param name="webhookName"><see cref="WebhookDefinition.Name"/></param>
        Task<bool> IsSubscribedAsync(int? tenantId, string webhookName);

        /// <summary>
        /// Checks if tenant subscribed for a webhook. (Checks if webhook features are granted)
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        /// <param name="webhookName"><see cref="WebhookDefinition.Name"/></param>
        bool IsSubscribed(int? tenantId, string webhookName);

        /// <summary>
        /// If id is the default(Guid) adds new subscription, else updates current one. (Checks if webhook features are granted)
        /// </summary>
        Task AddOrUpdateSubscriptionAsync(WebhookSubscription webhookSubscription);

        /// <summary>
        /// If id is the default(Guid) adds it, else updates it. (Checks if webhook features are granted)
        /// </summary>
        void AddOrUpdateSubscription(WebhookSubscription webhookSubscription);

        /// <summary>
        /// Activates/Deactivates given webhook subscription
        /// </summary>
        /// <param name="id">unique identifier of <see cref="WebhookSubscriptionInfo"/></param>
        /// <param name="active">IsActive</param>
        Task ActivateWebhookSubscriptionAsync(Guid id, bool active);

        /// <summary>
        /// Activates/Deactivates given webhook subscription
        /// </summary>
        /// <param name="id">unique identifier of <see cref="WebhookSubscriptionInfo"/></param>
        /// <param name="active">IsActive</param>
        void ActivateWebhookSubscription(Guid id, bool active);
    }
}
