using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Json;
using Castle.Core.Logging;

namespace Abp.Notifications
{
    /// <summary>
    /// Implements <see cref="INotificationManager"/>.
    /// </summary>
    public class NotificationManager : INotificationManager, ISingletonDependency
    {
        /// <summary>
        /// Reference to the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        private readonly INotificationStore _store;
        private readonly IBackgroundJobManager _backgroundJobManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationManager"/> class.
        /// </summary>
        public NotificationManager(
            INotificationStore store,
            IBackgroundJobManager backgroundJobManager)
        {
            _store = store;
            _backgroundJobManager = backgroundJobManager;

            Logger = NullLogger.Instance;
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

        [UnitOfWork]
        public virtual async Task PublishAsync(NotificationPublishOptions options)
        {
            var notificationInfo = new NotificationInfo
            {
                NotificationName = options.NotificationName,
                EntityTypeName = options.EntityType == null ? null : options.EntityType.FullName,
                EntityTypeAssemblyQualifiedName = options.EntityType == null ? null : options.EntityType.AssemblyQualifiedName,
                EntityId = options.EntityId == null ? null : options.EntityId.ToJsonString(),
                Severity = options.Severity,
                UserIds = options.UserIds.IsNullOrEmpty() ? null : options.UserIds.JoinAsString(","),
                Data = options.Data.ToJsonString(),
                DataTypeName = options.Data.GetType().AssemblyQualifiedName
            };

            await _store.InsertNotificationAsync(notificationInfo);
            
            await _backgroundJobManager.EnqueueAsync<NotificationDistributionJob, NotificationDistributionJobArgs>(
                new NotificationDistributionJobArgs(
                    notificationInfo.Id
                    )
                );
        }

        public Task<List<UserNotification>> GetUserNotifications(long userId, int skipCount = 0, int maxResultCount = Int32.MaxValue)
        {
            throw new NotImplementedException();
            //var userNotificationInfos = _store.GetUserNotifications(userId)
        }

        public Task<List<UserNotification>> GetUserNotification(Guid userNotificationId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserNotificationStateAsync(Guid userNotificationId, UserNotificationState state)
        {
            return _store.UpdateUserNotificationStateAsync(userNotificationId, state);
        }

        public Task UpdateAllUserNotificationStatesAsync(long userId, UserNotificationState state)
        {
            return _store.UpdateAllUserNotificationStatesAsync(userId, state);
        }

        public Task DeleteUserNotificationAsync(Guid userNotificationId)
        {
            return _store.DeleteUserNotificationAsync(userNotificationId);
        }

        public Task DeleteAllUserNotificationsAsync(long userId)
        {
            return _store.DeleteAllUserNotificationsAsync(userId);
        }
    }
}