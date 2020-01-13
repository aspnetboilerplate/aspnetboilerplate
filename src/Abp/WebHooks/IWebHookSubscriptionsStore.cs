using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.WebHooks
{
    /// <summary>
    /// This interface should be implemented by vendors to make webhooks working.
    /// </summary>
    public interface IWebHookSubscriptionsStore
    {
        /// <summary>
        /// returns subscription
        /// </summary>
        /// <param name="id">webhook subscription id</param>
        /// <returns></returns>
        Task<WebHookSubscriptionInfo> GetAsync(Guid id);

        /// <summary>
        /// returns subscription
        /// </summary>
        /// <param name="id">webhook subscription id</param>
        /// <returns></returns>
        WebHookSubscriptionInfo Get(Guid id);

        /// <summary>
        /// Saves webhook subscription to a persistent store.
        /// </summary>
        /// <param name="webHookSubscription">webhook subscription information</param>
        Task InsertAsync(WebHookSubscriptionInfo webHookSubscription);

        /// <summary>
        /// Saves webhook subscription to a persistent store.
        /// </summary>
        /// <param name="webHookSubscription">webhook subscription information</param>
        void Insert(WebHookSubscriptionInfo webHookSubscription);

        /// <summary>
        /// Updates webhook subscription to a persistent store.
        /// </summary>
        /// <param name="webHookSubscription">webhook subscription information</param>
        Task UpdateAsync(WebHookSubscriptionInfo webHookSubscription);

        /// <summary>
        /// Updates webhook subscription to a persistent store.
        /// </summary>
        /// <param name="webHookSubscription">webhook subscription information</param>
        void Update(WebHookSubscriptionInfo webHookSubscription);

        /// <summary>
        /// Deletes subscription if exists
        /// </summary>
        /// <param name="id"><see cref="WebHookSubscriptionInfo"/> primary key</param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Deletes subscription if exists 
        /// </summary>
        /// <param name="id"><see cref="WebHookSubscriptionInfo"/> primary key</param>
        /// <returns></returns>
        void Delete(Guid id);

        /// <summary>
        /// Returns all subscriptions of given tenant including deactivated 
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        Task<List<WebHookSubscriptionInfo>> GetAllSubscriptionsAsync(int? tenantId);

        /// <summary>
        /// Returns all subscriptions of given tenant including deactivated 
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        List<WebHookSubscriptionInfo> GetAllSubscriptions(int? tenantId);

        /// <summary>
        /// Returns webhook subscriptions which subscribe to given webhook on tenant(s)
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        /// <returns></returns>
        Task<List<WebHookSubscriptionInfo>> GetAllSubscriptionsAsync(int? tenantId, string webHookName);

        /// <summary>
        /// Returns webhook subscriptions which subscribe to given webhook on tenant(s)
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        /// <returns></returns>
        List<WebHookSubscriptionInfo> GetAllSubscriptions(int? tenantId, string webHookName);

        /// <summary>
        /// Checks if tenant subscribed for a webhook
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        /// <param name="webHookName">Name of the webhook</param>
        Task<bool> IsSubscribedAsync(int? tenantId, string webHookName);

        /// <summary>
        /// Checks if tenant subscribed for a webhook
        /// </summary>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        /// <param name="webHookName">Name of the webhook</param>
        bool IsSubscribed(int? tenantId, string webHookName);
    }
}
