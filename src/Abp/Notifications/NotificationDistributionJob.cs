using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Threading;

namespace Abp.Notifications
{
    public class NotificationDistributionJob : BackgroundJob<NotificationDistributionJobArgs>, ITransientDependency
    {
        public IRealTimeNotifier RealTimeNotifier { get; set; }

        private readonly INotificationStore _notificationStore;

        public NotificationDistributionJob(INotificationStore notificationStore)
        {
            _notificationStore = notificationStore;

            RealTimeNotifier = NullRealTimeNotifier.Instance;
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
                userIds = (await _notificationStore.GetSubscriptions(notification)).Select(s => s.UserId).ToArray();
            }
            else
            {
                userIds = notification.UserIds.Split(",").Select(uidAsStr => Convert.ToInt64(uidAsStr)).ToArray();
            }

            var userNotifications = userIds.Select(userId => new UserNotificationInfo(userId, notification.Id)).ToList();

            await SaveUserNotifications(userNotifications);

            try
            {
                await RealTimeNotifier.SendNotificationAsync(notification, userNotifications);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }
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
