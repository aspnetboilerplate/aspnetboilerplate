using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Json;

namespace Abp.Notifications
{
    /// <summary>
    /// Implements <see cref="INotificationSubscriptionManager"/>.
    /// </summary>
    public class NotificationSubscriptionManager : INotificationSubscriptionManager, ITransientDependency
    {
        private readonly INotificationStore _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSubscriptionManager"/> class.
        /// </summary>
        public NotificationSubscriptionManager(INotificationStore store)
        {
            _store = store;
        }

        public async Task SubscribeAsync(long userId, string notificationName, Type entityType = null, object entityId = null)
        {
            await _store.InsertSubscriptionAsync(
                new NotificationSubscriptionInfo(
                    userId,
                    notificationName,
                    entityType,
                    entityId
                    )
                );
        }

        public async Task UnsubscribeAsync(long userId, string notificationName, Type entityType = null, object entityId = null)
        {
            await _store.DeleteSubscriptionAsync(
                userId,
                notificationName,
                entityType == null ? null : entityType.FullName,
                entityId == null ? null : entityId.ToJsonString()
                );
        }

        public async Task<List<NotificationSubscription>> GetSubscriptionsAsync(string notificationName, Type entityType = null, object entityId = null)
        {
            var notificationSubscriptionInfos = await _store.GetSubscriptionsAsync(
                notificationName,
                entityType == null ? null : entityType.FullName,
                entityId == null ? null : entityId.ToJsonString()
                );

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public async Task<List<NotificationSubscription>> GetSubscribedNotificationsAsync(long userId)
        {
            var notificationSubscriptionInfos = await _store.GetSubscriptionsAsync(userId);

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public Task<bool> IsSubscribedAsync(long userId, string notificationName, Type entityType = null, object entityId = null)
        {
            return _store.IsSubscribedAsync(
                userId,
                notificationName,
                entityType == null ? null : entityType.FullName,
                entityId == null ? null : entityId.ToJsonString()
                );
        }
    }
}