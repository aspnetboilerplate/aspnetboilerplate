using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.WebHooks
{
    public interface IWebHookSubscriptionManager
    {
        /// <summary>
        /// Returns subscription for given id. 
        /// </summary>
        /// <param name="id">Unique identifier of <see cref="WebHookSubscriptionInfo"/></param>
        Task<WebHookSubscription> GetAsync(Guid id);

        /// <summary>
        /// Returns subscription for given id. 
        /// </summary>
        /// <param name="id">Unique identifier of <see cref="WebHookSubscriptionInfo"/></param>
        WebHookSubscription Get(Guid id);

        /// <summary>
        /// Returns all subscriptions of tenant
        /// </summary>
        /// <returns></returns>
        Task<List<WebHookSubscription>> GetAllSubscriptionsAsync(int? tenantId);

        /// <summary>
        /// Returns all subscriptions of tenant
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        List<WebHookSubscription> GetAllSubscriptions(int? tenantId);

        /// <summary>
        /// Returns all subscriptions for given webhook.
        /// </summary>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        Task<List<WebHookSubscription>> GetAllSubscriptionsIfFeaturesGrantedAsync(int? tenantId, string webHookName);

        /// <summary>
        /// Returns all subscriptions for given webhook.
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        List<WebHookSubscription> GetAllSubscriptionsIfFeaturesGranted(int? tenantId, string webHookName);

        /// <summary>
        /// Checks if tenant subscribed for a webhook. (Checks if webhook features are granted)
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        Task<bool> IsSubscribedAsync(int? tenantId, string webHookName);

        /// <summary>
        /// Checks if tenant subscribed for a webhook. (Checks if webhook features are granted)
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        bool IsSubscribed(int? tenantId, string webHookName);


        /// <summary>
        /// If id is the default(Guid) adds new subscription, else updates current one. (Checks if webhook features are granted)
        /// </summary>
        Task AddOrUpdateSubscriptionAsync(WebHookSubscription webHookSubscription);

        /// <summary>
        /// If id is the default(Guid) adds it, else updates it. (Checks if webhook features are granted)
        /// </summary>
        void AddOrUpdateSubscription(WebHookSubscription webHookSubscription);

    }
}
