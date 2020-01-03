using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.WebHooks
{
    /// <summary>
    /// This interface should be implemented by vendors to
    /// make webhooks working.
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
        /// Deletes subscription if exists with given id
        /// </summary>
        /// <param name="id">Subscription Id</param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Deletes subscription if exists with given id
        /// </summary>
        /// <param name="id">Subscription Id</param>
        /// <returns></returns>
        void Delete(Guid id);

        /// <summary>
        /// Returns webhook subscriptions which subscribe to given webhook
        /// </summary>
        /// <param name="webHookDefinitionName">webhook definition name</param>
        /// <returns></returns>
        Task<List<WebHookSubscriptionInfo>> GetAllSubscriptionsAsync(string webHookDefinitionName);

        /// <summary>
        /// Returns webhook subscriptions which subscribe to given webhook
        /// </summary>
        /// <param name="webHookDefinitionName">webhook definition name</param>
        /// <returns></returns>
        List<WebHookSubscriptionInfo> GetAllSubscriptions(string webHookDefinitionName);

        /// <summary>
        /// Checks if a user subscribed for a webhook
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="webHookName">Name of the webhook</param>
        Task<bool> IsSubscribedAsync(UserIdentifier user, string webHookName);

        /// <summary>
        /// Checks if a user subscribed for a webhook
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="webHookName">Name of the webhook</param>
        bool IsSubscribed(UserIdentifier user, string webHookName);

        /// <summary>
        /// Returns all subscriptions of given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<List<WebHookSubscriptionInfo>> GetAllSubscriptionsAsync(UserIdentifier user);

        /// <summary>
        /// Returns all subscriptions of given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        List<WebHookSubscriptionInfo> GetAllSubscriptions(UserIdentifier user);

        /// <summary>
        /// Change <see cref="WebHookSubscriptionInfo.IsActive"/> with given parameter
        /// </summary>
        /// <param name="id">WebHookSubscriptionInfo.Id</param>
        /// <param name="active">IsActive</param>
        Task SetActiveAsync(Guid id, bool active);

        /// <summary>
        /// Change <see cref="WebHookSubscriptionInfo.IsActive"/> with given parameter
        /// </summary>
        /// <param name="id">WebHookSubscriptionInfo.Id</param>
        /// <param name="active">IsActive</param>
        void SetActive(Guid id, bool active);

        /// <summary>
        /// Returns all subscriptions of given user which are subscribed to given webhook.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="webHookName"></param>
        Task<List<WebHookSubscriptionInfo>> GetAllSubscriptionsAsync(UserIdentifier user, string webHookName);

        /// <summary>
        /// Returns all subscriptions of given user which are subscribed to given webhook.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="webHookName"></param>
        List<WebHookSubscriptionInfo> GetAllSubscriptions(UserIdentifier user, string webHookName);
    }
}
