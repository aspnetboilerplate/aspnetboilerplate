using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Threading;
using Castle.Core.Internal;

namespace Abp.Notifications
{
    /// <summary>
    /// This background job distributes notifications to users.
    /// </summary>
    public class NotificationDistributionJob : BackgroundJob<NotificationDistributionJobArgs>, ITransientDependency
    {
        /// <summary>
        /// Referece to <see cref="IRealTimeNotifier"/>.
        /// </summary>
        public IRealTimeNotifier RealTimeNotifier { get; set; }

        private readonly INotificationStore _notificationStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDistributionJob"/> class.
        /// </summary>
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

            Notification notification;

            try
            {
                notification = notificationInfo.ToNotification();
            }
            catch (Exception)
            {
                Logger.Warn("NotificationDistributionJob can not continue since could not convert NotificationInfo to Notification for NotificationId: " + args.NotificationId);
                return;
            }

            var userIds = await GetUserIds(notificationInfo);

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
        protected virtual async Task<long[]> GetUserIds(NotificationInfo notificationInfo)
        {
            if (!notificationInfo.UserIds.IsNullOrEmpty())
            {
                return notificationInfo
                    .UserIds
                    .Split(",")
                    .Select(uidAsStr => uidAsStr.To<long>())
                    .ToArray();
            }

            var tenantIds = GetTenantIds(notificationInfo);

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                if (tenantIds.IsNullOrEmpty())
                {
                    return (await _notificationStore.GetSubscriptionsAsync(
                        notificationInfo.NotificationName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityId
                        )).Select(s => s.UserId)
                        .ToArray();
                }
                else
                {
                    return (await _notificationStore.GetSubscriptionsAsync(
                        tenantIds,
                        notificationInfo.NotificationName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityId
                        )).Select(s => s.UserId)
                        .ToArray();
                }
            }
        }

        private static int?[] GetTenantIds(NotificationInfo notificationInfo)
        {
            if (notificationInfo.TenantIds.IsNullOrEmpty())
            {
                return null;
            }

            return notificationInfo
                .TenantIds
                .Split(",")
                .Select(uidAsStr => uidAsStr == "null" ? (int?)null : (int?)uidAsStr.To<int>())
                .ToArray();
        }

        [UnitOfWork]
        protected virtual async Task SaveUserNotifications(IEnumerable<UserNotificationInfo> userNotifications)
        {
            foreach (var userNotification in userNotifications)
            {
                await _notificationStore.InsertUserNotificationAsync(userNotification);
            }

            await CurrentUnitOfWork.SaveChangesAsync(); //To get Ids of the notifications
        }
    }
}
