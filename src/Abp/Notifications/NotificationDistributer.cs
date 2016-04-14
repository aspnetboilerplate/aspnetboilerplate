using Abp.Configuration;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Extensions;
using Castle.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to distribute notifications to users.
    /// </summary>
    public class NotificationDistributer : DomainService, INotificationDistributer
    {
        /// <summary>
        /// Referece to <see cref="IRealTimeNotifier"/>.
        /// </summary>
        public IRealTimeNotifier RealTimeNotifier { get; set; }

        private readonly INotificationDefinitionManager _notificationDefinitionManager;
        private readonly INotificationStore _notificationStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDistributionJob"/> class.
        /// </summary>
        public NotificationDistributer(
            INotificationDefinitionManager notificationDefinitionManager,
            INotificationStore notificationStore)
        {
            _notificationDefinitionManager = notificationDefinitionManager;
            _notificationStore = notificationStore;

            RealTimeNotifier = NullRealTimeNotifier.Instance;
        }

        public async Task DistributeAsync(Guid notificationId)
        {
            var notificationInfo = await _notificationStore.GetNotificationOrNullAsync(notificationId);
            if (notificationInfo == null)
            {
                Logger.Warn("NotificationDistributionJob can not continue since could not found notification by id: " + notificationId);
                return;
            }

            Notification notification;

            try
            {
                notification = notificationInfo.ToNotification();
            }
            catch (Exception)
            {
                Logger.Warn("NotificationDistributionJob can not continue since could not convert NotificationInfo to Notification for NotificationId: " + notificationId);
                return;
            }

            var userIds = await GetUserIds(notificationInfo);

            var userNotificationInfos = userIds
                .Select(userId => new UserNotificationInfo(userId, notificationInfo.Id))
                .ToList();

            await SaveUserNotifications(userNotificationInfos);

            try
            {
                var userNotifications = userNotificationInfos
                    .Select(uni => uni.ToUserNotification(notification))
                    .ToArray();

                await RealTimeNotifier.SendNotificationsAsync(userNotifications);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }
        }

        [UnitOfWork]
        protected virtual async Task<Guid[]> GetUserIds(NotificationInfo notificationInfo)
        {
            List<Guid> userIds;

            if (!notificationInfo.UserIds.IsNullOrEmpty())
            {
                //Directly get from UserIds
                userIds = notificationInfo
                    .UserIds
                    .Split(",")
                    .Select(uidAsStr => Guid.Parse(uidAsStr))
                    .Where(uid => SettingManager.GetSettingValueForUser<bool>(NotificationSettingNames.ReceiveNotifications, null, uid))
                    .ToList();
            }
            else
            {
                //Get subscribed users

                var tenantIds = GetTenantIds(notificationInfo);

                List<NotificationSubscriptionInfo> subscriptions;

                if (tenantIds.IsNullOrEmpty())
                {
                    //Get all subscribed users of all tenants
                    subscriptions = await _notificationStore.GetSubscriptionsAsync(
                        notificationInfo.NotificationName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityId
                        );
                }
                else
                {
                    //Get all subscribed users of specified tenant(s)
                    subscriptions = await _notificationStore.GetSubscriptionsAsync(
                        tenantIds,
                        notificationInfo.NotificationName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityId
                        );
                }

                //Remove invalid subscriptions
                var invalidSubscriptions = new Dictionary<Guid, NotificationSubscriptionInfo>();

                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    foreach (var subscription in subscriptions)
                    {
                        if (!await _notificationDefinitionManager.IsAvailableAsync(notificationInfo.NotificationName, subscription.TenantId, subscription.UserId) ||
                            !SettingManager.GetSettingValueForUser<bool>(NotificationSettingNames.ReceiveNotifications, subscription.TenantId, subscription.UserId))
                        {
                            invalidSubscriptions[subscription.Id] = subscription;
                        }
                    }
                }

                subscriptions.RemoveAll(s => invalidSubscriptions.ContainsKey(s.Id));

                //Get user ids
                userIds = subscriptions
                    .Select(s => s.UserId)
                    .ToList();
            }

            if (!notificationInfo.ExcludedUserIds.IsNullOrEmpty())
            {
                //Exclude specified users.
                var excludedUserIds = notificationInfo
                    .ExcludedUserIds
                    .Split(",")
                    .Select(uidAsStr => uidAsStr.To<Guid>())
                    .ToList();

                userIds.RemoveAll(uid => excludedUserIds.Contains(uid));
            }

            return userIds.ToArray();
        }

        private static Guid?[] GetTenantIds(NotificationInfo notificationInfo)
        {
            if (notificationInfo.TenantIds.IsNullOrEmpty())
            {
                return null;
            }

            return notificationInfo
                .TenantIds
                .Split(",")
                .Select(uidAsStr => uidAsStr == "null" ? (Guid?)null : (Guid?)uidAsStr.To<Guid>())
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