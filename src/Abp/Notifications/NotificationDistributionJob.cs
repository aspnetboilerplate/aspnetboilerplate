using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Threading;

namespace Abp.Notifications
{
    public class NotificationDistributionJob : BackgroundJob<NotificationDistributionJobArgs>, ITransientDependency
    {
        private readonly INotificationStore _notificationStore;
        private readonly IRealTimeNotifier _realTimeNotifier;

        public NotificationDistributionJob(INotificationStore notificationStore, IRealTimeNotifier realTimeNotifier)
        {
            _notificationStore = notificationStore;
            _realTimeNotifier = realTimeNotifier;
        }

        public override void Execute(NotificationDistributionJobArgs args)
        {
            AsyncHelper.RunSync(() => ExecuteAsync(args));
        }

        private async Task ExecuteAsync(NotificationDistributionJobArgs args)
        {
            var notification = await _notificationStore.GetNotificationOrNullAsync(args.NotificationId);
            if (notification == null)
            {
                Logger.Warn("NotificationDistributionJob can not continue since could not found notification by id: " + args.NotificationId);
                return;
            }

            long[] userIds;
            if (notification.UserIds.IsNullOrEmpty())
            {
                userIds = await _notificationStore.GetSubscribedUserIdsAsync(notification);
            }
            else
            {
                userIds = notification.UserIds.Split(",").Select(uidAsStr => Convert.ToInt64(uidAsStr)).ToArray();
            }

            var userNotifications = userIds.Select(userId => new UserNotificationInfo(userId, notification.Id)).ToList();

            await SaveUserNotifications(userNotifications);

            await _realTimeNotifier.SendNotificationAsync(notification, userNotifications);
        }

        [UnitOfWork]
        protected virtual async Task SaveUserNotifications(IEnumerable<UserNotificationInfo> userNotifications)
        {
            foreach (var userNotification in userNotifications)
            {
                await _notificationStore.InsertUserNotificationAsync(userNotification);
            }
        }
    }
}
