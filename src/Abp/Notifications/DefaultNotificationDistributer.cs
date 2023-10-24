using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Extensions;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to distribute notifications to users.
    /// </summary>
    public class DefaultNotificationDistributer : DomainService, INotificationDistributer
    {
        private readonly INotificationConfiguration _notificationConfiguration;
        private readonly INotificationDefinitionManager _notificationDefinitionManager;
        private readonly INotificationStore _notificationStore;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDistributionJob"/> class.
        /// </summary>
        public DefaultNotificationDistributer(
            INotificationConfiguration notificationConfiguration,
            INotificationDefinitionManager notificationDefinitionManager,
            INotificationStore notificationStore,
            IUnitOfWorkManager unitOfWorkManager,
            IGuidGenerator guidGenerator,
            IIocResolver iocResolver)
        {
            _notificationConfiguration = notificationConfiguration;
            _notificationDefinitionManager = notificationDefinitionManager;
            _notificationStore = notificationStore;
            _unitOfWorkManager = unitOfWorkManager;
            _guidGenerator = guidGenerator;
            _iocResolver = iocResolver;
        }

        public virtual async Task DistributeAsync(Guid notificationId)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var notificationInfo = await _notificationStore.GetNotificationOrNullAsync(notificationId);
                if (notificationInfo == null)
                {
                    Logger.Warn(
                        "NotificationDistributionJob can not continue since could not found notification by id: " +
                        notificationId
                    );

                    return;
                }

                var users = await GetUsersAsync(notificationInfo);

                var userNotifications = await SaveUserNotificationsAsync(users, notificationInfo);

                await _notificationStore.DeleteNotificationAsync(notificationInfo);

                await NotifyAsync(userNotifications.ToArray());
            });
        }

        protected virtual async Task<UserIdentifier[]> GetUsersAsync(NotificationInfo notificationInfo)
        {
            List<UserIdentifier> userIds;

            if (!notificationInfo.UserIds.IsNullOrEmpty())
            {
                // Directly get from UserIds
                userIds = notificationInfo
                    .UserIds
                    .Split(",")
                    .Select(UserIdentifier.Parse)
                    .Where(uid =>
                        SettingManager.GetSettingValueForUser<bool>(NotificationSettingNames.ReceiveNotifications,
                            uid.TenantId, uid.UserId))
                    .ToList();
            }
            else
            {
                var tenantIds = GetTenantIds(notificationInfo);

                List<NotificationSubscriptionInfo> subscriptions;

                if (tenantIds.IsNullOrEmpty() ||
                    (tenantIds.Length == 1 && tenantIds[0] == NotificationInfo.AllTenantIds.To<int>()))
                {
                    // Get all subscribed users of all tenants
                    subscriptions = await _notificationStore.GetSubscriptionsAsync(
                        notificationInfo.NotificationName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityId,
                        notificationInfo.TargetNotifiers
                    );
                }
                else
                {
                    // Get all subscribed users of specified tenant(s)
                    subscriptions = await _notificationStore.GetSubscriptionsAsync(
                        tenantIds,
                        notificationInfo.NotificationName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityId,
                        notificationInfo.TargetNotifiers
                    );
                }

                // Remove invalid subscriptions
                var invalidSubscriptions = new Dictionary<Guid, NotificationSubscriptionInfo>();

                // TODO: Group subscriptions per tenant for potential performance improvement
                foreach (var subscription in subscriptions)
                {
                    using (CurrentUnitOfWork.SetTenantId(subscription.TenantId))
                    {
                        if (!await _notificationDefinitionManager.IsAvailableAsync(notificationInfo.NotificationName,
                                new UserIdentifier(subscription.TenantId, subscription.UserId)) ||
                            !SettingManager.GetSettingValueForUser<bool>(NotificationSettingNames.ReceiveNotifications,
                                subscription.TenantId, subscription.UserId))
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

        protected virtual UserIdentifier[] GetUsers(NotificationInfo notificationInfo)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                List<UserIdentifier> userIds;

                if (!notificationInfo.UserIds.IsNullOrEmpty())
                {
                    //Directly get from UserIds
                    userIds = notificationInfo
                        .UserIds
                        .Split(",")
                        .Select(uidAsStr => UserIdentifier.Parse(uidAsStr))
                        .Where(uid =>
                            SettingManager.GetSettingValueForUser<bool>(NotificationSettingNames.ReceiveNotifications,
                                uid.TenantId, uid.UserId))
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
                        subscriptions = _notificationStore.GetSubscriptions(
                            notificationInfo.NotificationName,
                            notificationInfo.EntityTypeName,
                            notificationInfo.EntityId,
                            notificationInfo.TargetNotifiers
                        );
                    }
                    else
                    {
                        //Get all subscribed users of specified tenant(s)
                        subscriptions = _notificationStore.GetSubscriptions(
                            tenantIds,
                            notificationInfo.NotificationName,
                            notificationInfo.EntityTypeName,
                            notificationInfo.EntityId,
                            notificationInfo.TargetNotifiers
                        );
                    }

                    //Remove invalid subscriptions
                    var invalidSubscriptions = new Dictionary<Guid, NotificationSubscriptionInfo>();

                    //TODO: Group subscriptions per tenant for potential performance improvement
                    foreach (var subscription in subscriptions)
                    {
                        using (CurrentUnitOfWork.SetTenantId(subscription.TenantId))
                        {
                            if (!_notificationDefinitionManager.IsAvailable(notificationInfo.NotificationName,
                                    new UserIdentifier(subscription.TenantId, subscription.UserId)) ||
                                !SettingManager.GetSettingValueForUser<bool>(
                                    NotificationSettingNames.ReceiveNotifications, subscription.TenantId,
                                    subscription.UserId))
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
            });
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
                .Select(tenantIdAsStr => tenantIdAsStr == "null" ? (int?) null : (int?) tenantIdAsStr.To<int>())
                .ToArray();
        }

        protected virtual async Task<List<UserNotification>> SaveUserNotificationsAsync(
            UserIdentifier[] users,
            NotificationInfo notificationInfo)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var userNotifications = new List<UserNotification>();

                var tenantGroups = users.GroupBy(user => user.TenantId);
                foreach (var tenantGroup in tenantGroups)
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantGroup.Key))
                    {
                        var tenantNotificationInfo = new TenantNotificationInfo(
                            _guidGenerator.Create(),
                            tenantGroup.Key,
                            notificationInfo
                        );

                        await _notificationStore.InsertTenantNotificationAsync(tenantNotificationInfo);
                        await _unitOfWorkManager.Current.SaveChangesAsync(); //To get tenantNotification.Id.

                        var tenantNotification = tenantNotificationInfo.ToTenantNotification();

                        var userNotificationSubscriptions = await _notificationStore.GetSubscriptionsAsync(
                            notificationInfo.NotificationName,
                            notificationInfo.EntityTypeName,
                            notificationInfo.EntityId,
                            null
                        );

                        foreach (var user in tenantGroup)
                        {
                            var userNotification = new UserNotificationInfo(_guidGenerator.Create())
                            {
                                TenantId = tenantGroup.Key,
                                UserId = user.UserId,
                                TenantNotificationId = tenantNotificationInfo.Id,
                                TargetNotifiers = GetTargetNotifiersForUser(
                                    user,
                                    notificationInfo,
                                    userNotificationSubscriptions
                                )
                            };

                            await _notificationStore.InsertUserNotificationAsync(userNotification);
                            userNotifications.Add(userNotification.ToUserNotification(tenantNotification));
                        }

                        await CurrentUnitOfWork.SaveChangesAsync(); //To get Ids of the notifications
                    }
                }

                return userNotifications;
            });
        }

        protected virtual string GetTargetNotifiersForUser(
            UserIdentifier user,
            NotificationInfo notificationInfo,
            List<NotificationSubscriptionInfo> userNotificationSubscriptions
        )
        {
            if (userNotificationSubscriptions.IsNullOrEmpty())
            {
                return notificationInfo.TargetNotifiers;
            }

            var userSubscription = userNotificationSubscriptions.FirstOrDefault(un => un.UserId == user.UserId);
            if (userSubscription == null)
            {
                return notificationInfo.TargetNotifiers;
            }

            return userSubscription.TargetNotifiers;
        }

        protected virtual List<UserNotification> SaveUserNotifications(
            UserIdentifier[] users,
            NotificationInfo notificationInfo)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var userNotifications = new List<UserNotification>();

                var tenantGroups = users.GroupBy(user => user.TenantId);
                foreach (var tenantGroup in tenantGroups)
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantGroup.Key))
                    {
                        var tenantNotificationInfo = new TenantNotificationInfo(_guidGenerator.Create(),
                            tenantGroup.Key, notificationInfo);
                        _notificationStore.InsertTenantNotification(tenantNotificationInfo);
                        _unitOfWorkManager.Current.SaveChanges(); //To get tenantNotification.Id.

                        var tenantNotification = tenantNotificationInfo.ToTenantNotification();

                        foreach (var user in tenantGroup)
                        {
                            var userNotification = new UserNotificationInfo(_guidGenerator.Create())
                            {
                                TenantId = tenantGroup.Key,
                                UserId = user.UserId,
                                TenantNotificationId = tenantNotificationInfo.Id
                            };

                            _notificationStore.InsertUserNotification(userNotification);
                            userNotifications.Add(userNotification.ToUserNotification(tenantNotification));
                        }

                        CurrentUnitOfWork.SaveChanges(); //To get Ids of the notifications
                    }
                }

                return userNotifications;
            });
        }

        #region Protected methods

        protected virtual async Task NotifyAsync(UserNotification[] userNotifications)
        {
            foreach (var notifierType in _notificationConfiguration.Notifiers)
            {
                try
                {
                    using (var notifier = _iocResolver.ResolveAsDisposable<IRealTimeNotifier>(notifierType))
                    {
                        UserNotification[] notificationsToSendWithThatNotifier;

                        // if UseOnlyIfRequestedAsTarget is true, then we should send notifications which requests this notifier
                        if (notifier.Object.UseOnlyIfRequestedAsTarget)
                        {
                            notificationsToSendWithThatNotifier = userNotifications
                                .Where(n => n.TargetNotifiersList.Contains(notifierType.FullName))
                                .ToArray();
                        }
                        else
                        {
                            // notifier allows to send any notifications 
                            // we can send all notifications which does not have TargetNotifiersList(since there is no target, we can send it with any notifier)
                            // or current notifier is in TargetNotifiersList

                            notificationsToSendWithThatNotifier = userNotifications
                                .Where(n =>
                                        n.TargetNotifiersList == null ||
                                        n.TargetNotifiersList.Count ==
                                        0 || // if there is no target notifiers, send it to all of them
                                        n.TargetNotifiersList.Contains(notifierType
                                            .FullName) // if there is target notifiers, check if current notifier is in it
                                )
                                .ToArray();
                        }

                        if (notificationsToSendWithThatNotifier.Length == 0)
                        {
                            continue;
                        }

                        await notifier.Object.SendNotificationsAsync(notificationsToSendWithThatNotifier);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString(), ex);
                }
            }
        }

        #endregion
    }
}