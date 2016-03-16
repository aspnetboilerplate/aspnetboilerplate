using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Threading;
using Newtonsoft.Json;

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
            var notificationInfo = await _notificationStore.GetNotificationOrNullAsync(args.NotificationId);
            if (notificationInfo == null)
            {
                Logger.Warn("NotificationDistributionJob can not continue since could not found notification by id: " + args.NotificationId);
                return;
            }

            var notification = notificationInfo.ToNotification(); //TODO: Handle exceptions?

            long[] userIds;
            if (notificationInfo.UserIds.IsNullOrEmpty())
            {
                userIds = (await _notificationStore.GetSubscriptions(notificationInfo)).Select(s => s.UserId).ToArray();
            }
            else
            {
                userIds = notificationInfo.UserIds.Split(",").Select(uidAsStr => Convert.ToInt64(uidAsStr)).ToArray();
            }

            var userNotificationInfos = userIds.Select(userId => new UserNotificationInfo(userId, notificationInfo.Id)).ToList();

            await SaveUserNotifications(userNotificationInfos);

            try
            {
                var userNotifications = userNotificationInfos.Select(uni => uni.ToUserNotification(notification)).ToArray();
                await RealTimeNotifier.SendNotificationsAsync(userNotifications);
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
