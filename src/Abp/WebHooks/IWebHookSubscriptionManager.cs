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
        /// Returns all subscriptions of given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<List<WebHookSubscription>> GetAllSubscriptionsAsync(UserIdentifier user);

        /// <summary>
        /// Returns all subscriptions of given user
        /// </summary>
        /// <param name="user"></param>
        List<WebHookSubscription> GetAllSubscriptions(UserIdentifier user);

        /// <summary>
        /// Returns all subscriptions of given user which are subscribed to given webhook. If user does not have permission to subscribe given webhook, returns empty
        /// </summary>
        /// <param name="user"></param>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        Task<List<WebHookSubscription>> GetAllSubscriptionsPermissionGrantedAsync(UserIdentifier user, string webHookName);

        /// <summary>
        /// Returns all subscriptions of given user which are subscribed to given webhook. If user does not have permission to subscribe given webhook, returns empty
        /// </summary>
        /// <param name="user"></param>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        List<WebHookSubscription> GetAllSubscriptionsPermissionGranted(UserIdentifier user, string webHookName);

        /// <summary>
        /// Returns all subscriptions for given webhook. Includes only users who has permissions to webhook
        /// </summary>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        Task<List<WebHookSubscription>> GetAllSubscriptionsPermissionGrantedAsync(string webHookName);

        /// <summary>
        /// Returns all subscriptions for given webhook. Includes only users who has permissions to webhook
        /// </summary>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        List<WebHookSubscription> GetAllSubscriptionsPermissionGranted(string webHookName);

        /// <summary>
        /// Checks if a user subscribed for a webhook
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        Task<bool> IsSubscribedAsync(UserIdentifier user, string webHookName);

        /// <summary>
        /// Checks if a user subscribed for a webhook
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        bool IsSubscribed(UserIdentifier user, string webHookName);

        /// <summary>
        /// Activates subscription
        /// </summary>
        /// <param name="id">Unique identifier of <see cref="WebHookSubscriptionInfo"/></param>
        /// <returns></returns>
        Task ActivateSubscriptionAsync(Guid id);

        /// <summary>
        /// Activates subscription
        /// </summary>
        /// <param name="id">Unique identifier of <see cref="WebHookSubscriptionInfo"/></param>
        void ActivateSubscription(Guid id);

        /// <summary>
        /// Deactivates subscription
        /// </summary>
        /// <param name="id">Unique identifier of <see cref="WebHookSubscriptionInfo"/></param>
        Task DeactivateSubscriptionAsync(Guid id);

        /// <summary>
        /// Deactivates subscription
        /// </summary>
        /// <param name="id">Unique identifier of <see cref="WebHookSubscriptionInfo"/></param>
        void DeactivateSubscription(Guid id);

        /// <summary>
        /// If id is the default(Guid) adds it, else updates it. (Checks if webhook permissions are granted)
        /// </summary>
        Task AddOrUpdateSubscriptionAsync(WebHookSubscription webHookSubscription);

        /// <summary>
        /// If id is the default(Guid) adds it, else updates it. (Checks if webhook permissions are granted)
        /// </summary>
        void AddOrUpdateSubscription(WebHookSubscription webHookSubscription);

        /// <summary>
        /// Adds subscription. (Checks if webhook permissions are granted)
        /// </summary>
        /// <param name="id">Unique identifier of <see cref="WebHookSubscriptionInfo"/></param>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        Task AddWebHookAsync(Guid id, string webHookName);

        /// <summary>
        /// Adds subscription. (Checks if webhook permissions are granted)
        /// </summary>
        /// <param name="id">Unique identifier of <see cref="WebHookSubscriptionInfo"/></param>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        void AddWebHook(Guid id, string webHookName);
    }
}
