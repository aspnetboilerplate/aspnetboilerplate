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
        Task<WebHookSubscription> GetAsync(Guid id);

        /// <summary>
        /// Returns subscription for given id. 
        /// </summary>
        WebHookSubscription Get(Guid id);

        /// <summary>
        /// Returns all subscriptions for given webhook. 
        /// </summary>
        /// <param name="webHookName">Name of the webhook</param>
        Task<List<WebHookSubscription>> GetAllSubscriptionsAsync(string webHookName);

        /// <summary>
        /// Returns all subscriptions for given webhook. 
        /// </summary>
        /// <param name="webHookName">Name of the webhook</param>
        List<WebHookSubscription> GetAllSubscriptions(string webHookName);

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
        /// Activates subscription
        /// </summary>
        /// <param name="id">Subscription Id</param>
        /// <returns></returns>
        Task ActivateSubscriptionAsync(Guid id);

        /// <summary>
        /// Activates subscription
        /// </summary>
        /// <param name="id">Subscription Id</param>
        void ActivateSubscription(Guid id);

        /// <summary>
        /// Deactivates subscription
        /// </summary>
        /// <param name="id">Subscription Id</param>
        Task DeactivateSubscriptionAsync(Guid id);

        /// <summary>
        /// Deactivates subscription
        /// </summary>
        /// <param name="id">Subscription Id</param>
        void DeactivateSubscription(Guid id);

        /// <summary>
        /// If id is default(Guid) adds it, else update it
        /// </summary>
        Task AddOrUpdateSubscriptionAsync(WebHookSubscription webHookSubscription);

        /// <summary>
        /// If id is default(Guid) adds it, else update it
        /// </summary>
        void AddOrUpdateSubscription(WebHookSubscription webHookSubscription);

        /// <summary>
        /// Adds subscription
        /// </summary>
        Task AddWebHookAsync(Guid id, string webHookName);

        /// <summary>
        /// Adds subscription
        /// </summary>
        void AddWebHook(Guid id, string webHookName);

        /// <summary>
        /// Returns all subscriptions of given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<List<WebHookSubscription>> GetAllSubscriptionsAsync(UserIdentifier user);

        /// <summary>
        /// Returns all subscriptions of given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        List<WebHookSubscription> GetAllSubscriptions(UserIdentifier user);
    }
}
