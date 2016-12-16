using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Extensions;
using Castle.Core.Internal;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to distribute notifications to users.
    /// </summary>
    public class NotificationDistributer : DomainService, INotificationDistributer
    {
        public IRealTimeNotifier RealTimeNotifier { get; set; }

        private readonly INotificationDefinitionManager _notificationDefinitionManager;
        private readonly INotificationStore _notificationStore;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDistributionJob"/> class.
        /// </summary>
        public NotificationDistributer(
            INotificationDefinitionManager notificationDefinitionManager,
            INotificationStore notificationStore,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _notificationDefinitionManager = notificationDefinitionManager;
            _notificationStore = notificationStore;
            _unitOfWorkManager = unitOfWorkManager;

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

            var users = await GetUsers(notificationInfo);

            var userNotifications = await SaveUserNotifications(users, notificationInfo);

            await _notificationStore.DeleteNotificationAsync(notificationInfo);

            try
            {
                await RealTimeNotifier.SendNotificationsAsync(userNotifications.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }
        }

        [UnitOfWork]
        protected virtual async Task<UserIdentifier[]> GetUsers(NotificationInfo notificationInfo)
        {
            List<UserIdentifier> userIds;

            if (!notificationInfo.UserIds.IsNullOrEmpty())
            {
                //Directly get from UserIds
                userIds = notificationInfo
                    .UserIds
                    .Split(",")
                    .Select(uidAsStr => UserIdentifier.Parse(uidAsStr))
                    .Where(uid => SettingManager.GetSettingValueForUser<bool>(NotificationSettingNames.ReceiveNotifications, uid.TenantId, uid.UserId))
                    .ToList();
            }
            else
            {
                //Get subscribed users

                var tenantIds = GetTenantIds(notificationInfo);

                List<NotificationSubscriptionInfo> subscriptions;

                if (tenantIds.IsNullOrEmpty() ||
                    (tenantIds.Length == 1 && tenantIds[0] == NotificationInfo.AllTenantIds.To<int>()))
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

                //TODO: Group subscriptions per tenant for potential performance improvement
                foreach (var subscription in subscriptions)
                {
                    using (CurrentUnitOfWork.SetTenantId(subscription.TenantId))
                    {
                        if (!await _notificationDefinitionManager.IsAvailableAsync(notificationInfo.NotificationName, new UserIdentifier(subscription.TenantId, subscription.UserId)) ||
                            !SettingManager.GetSettingValueForUser<bool>(NotificationSettingNames.ReceiveNotifications, subscription.TenantId, subscription.UserId))
                        {
                            invalidSubscriptions[subscription.Id] = subscription;
                        }
                    }
                }

                subscriptions.RemoveAll(s => invalidSubscriptions.ContainsKey(s.Id));

                //Get user ids
                userIds = subscriptions
                    .Select(s => new UserIdentifier(s.TenantId, s.UserId))
                    .ToList();
            }

            if (!notificationInfo.ExcludedUserIds.IsNullOrEmpty())
            {
                //Exclude specified users.
                var excludedUserIds = notificationInfo
                    .ExcludedUserIds
                    .Split(",")
                    .Select(uidAsStr => UserIdentifier.Parse(uidAsStr))
                    .ToList();

                userIds.RemoveAll(uid => excludedUserIds.Any(euid => euid.Equals(uid)));
            }

            return userIds.ToArray();
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
                .Select(tenantIdAsStr => tenantIdAsStr == "null" ? (int?)null : (int?)tenantIdAsStr.To<int>())
                .ToArray();
        }

        [UnitOfWork]
        protected virtual async Task<List<UserNotification>> SaveUserNotifications(UserIdentifier[] users, NotificationInfo notificationInfo)
        {
            var userNotifications = new List<UserNotification>();

            var tenantGroups = users.GroupBy(user => user.TenantId);
            foreach (var tenantGroup in tenantGroups)
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantGroup.Key))
                {
                    var tenantNotificationInfo = new TenantNotificationInfo(tenantGroup.Key, notificationInfo);
                    await _notificationStore.InsertTenantNotificationAsync(tenantNotificationInfo);
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get tenantNotification.Id.

                    var tenantNotification = tenantNotificationInfo.ToTenantNotification();

                    foreach (var user in tenantGroup)
                    {
                        var userNotification = new UserNotificationInfo
                        {
                            TenantId = tenantGroup.Key,
                            UserId = user.UserId,
                            TenantNotificationId = tenantNotificationInfo.Id
                        };

                        await _notificationStore.InsertUserNotificationAsync(userNotification);
                        userNotifications.Add(userNotification.ToUserNotification(tenantNotification));
                    }

                    await CurrentUnitOfWork.SaveChangesAsync(); //To get Ids of the notifications
                }
            }

            return userNotifications;
        }
    }
}