using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Castle.Core.Logging;

namespace Abp.Notifications
{
    public class UserNotificationQueue : IUserNotificationQueue, ISingletonDependency
    {
        public ILogger Logger { get; set; }

        private readonly Queue<UserNotificationQueueItem> _queue;

        private readonly INotificationStore _store;
        private readonly INotificationDefinitionManager _notificationDefinitionManager;
        private readonly IRealTimeNotifier _realTimeNotifier;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserNotificationQueue(
            INotificationStore store, 
            INotificationDefinitionManager notificationDefinitionManager, 
            IUnitOfWorkManager unitOfWorkManager, 
            IRealTimeNotifier realTimeNotifier)
        {
            _store = store;
            _notificationDefinitionManager = notificationDefinitionManager;
            _unitOfWorkManager = unitOfWorkManager;
            _realTimeNotifier = realTimeNotifier;

            Logger = NullLogger.Instance;
        }

        public void Add(NotificationInfo notification)
        {
            _queue.Enqueue(new UserNotificationQueueItem(notification));
            //TODO: Trigger processing
        }

        [UnitOfWork(TransactionScopeOption.RequiresNew)]
        protected virtual async Task ProcessQueueItemAsync(UserNotificationQueueItem queueItem)
        {
            try
            {

                var notificationDefinition = _notificationDefinitionManager.Get(queueItem.Notification.NotificationName);
                var userIds = queueItem.Notification.UserIds.IsNullOrEmpty() ?
                    await _store.GetSubscribedUserIds(queueItem.Notification.NotificationName)
                    : queueItem.Notification.UserIds;
                await _store.InsertUserNotificationAsync(new UserNotificationInfo());

                //TODO: Set notification as distributed.

                await _realTimeNotifier.SendNotificationAsync(userIds, queueItem.Notification).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //TODO: Set as failed, or re-try later?
            }
        }
    }

    [Serializable]
    public class UserNotificationQueueItem
    {
        public NotificationInfo Notification { get; set; }

        public UserNotificationQueueItem(NotificationInfo notification)
        {
            Notification = notification;
        }
    }
}