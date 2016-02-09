using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Json;

namespace Abp.Notifications
{
    /// <summary>
    /// Implements <see cref="INotificationSubscriptionManager"/>.
    /// </summary>
    public class NotificationSubscriptionManager : INotificationSubscriptionManager, ITransientDependency
    {
        private readonly INotificationStore _store;
        private readonly INotificationDefinitionManager _notificationDefinitionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSubscriptionManager"/> class.
        /// </summary>
        public NotificationSubscriptionManager(INotificationStore store, INotificationDefinitionManager notificationDefinitionManager)
        {
            _store = store;
            _notificationDefinitionManager = notificationDefinitionManager;
        }

        public async Task SubscribeAsync(int? tenantId, long userId, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            if (await IsSubscribedAsync(userId, notificationName, entityIdentifier))
            {
                return;
            }

            await _store.InsertSubscriptionAsync(
                new NotificationSubscriptionInfo(
                    tenantId,
                    userId,
                    notificationName,
                    entityIdentifier
                    )
                );
        }

        public async Task SubscribeToAllAvailableNotificationsAsync(int? tenantId, long userId)
        {
            var notificationDefinitions = (await _notificationDefinitionManager
                .GetAllAvailableAsync(tenantId, userId))
                .Where(nd => nd.EntityType == null)
                .ToList();

            foreach (var notificationDefinition in notificationDefinitions)
            {
                await SubscribeAsync(tenantId, userId, notificationDefinition.Name);
            }
        }

        public async Task UnsubscribeAsync(long userId, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            await _store.DeleteSubscriptionAsync(
                userId,
                notificationName,
                entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                entityIdentifier == null ? null : entityIdentifier.Id.ToJsonString()
                );
        }

        public async Task<List<NotificationSubscription>> GetSubscriptionsAsync(string notificationName, EntityIdentifier entityIdentifier = null)
        {
            var notificationSubscriptionInfos = await _store.GetSubscriptionsAsync(
                notificationName,
                entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                entityIdentifier == null ? null : entityIdentifier.Id.ToJsonString()
                );

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public async Task<List<NotificationSubscription>> GetSubscriptionsAsync(int? tenantId, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            var notificationSubscriptionInfos = await _store.GetSubscriptionsAsync(
                new[] { tenantId },
                notificationName,
                entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                entityIdentifier == null ? null : entityIdentifier.Id.ToJsonString()
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

        public Task<bool> IsSubscribedAsync(long userId, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            return _store.IsSubscribedAsync(
                userId,
                notificationName,
                entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                entityIdentifier == null ? null : entityIdentifier.Id.ToJsonString()
                );
        }
    }
}