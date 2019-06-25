using System;
using System.Threading;
using System.Threading.Tasks;
using Abp.Notifications;
using Abp.Runtime.Session;
using Shouldly;
using Xunit;

namespace Abp.Zero.Notifications
{
    public class NotificationStore_Tests : AbpZeroTestBase
    {
        private readonly INotificationStore _notificationStore;
        private readonly INotificationPublisher _notificationPublisher;

        public NotificationStore_Tests()
        {
            _notificationPublisher = LocalIocManager.Resolve<INotificationPublisher>();
            _notificationStore = LocalIocManager.Resolve<INotificationStore>();
        }
        [Fact]
        public async Task Should_Get_All_Notifications()
        {

            var userIdentifier = AbpSession.ToUserIdentifier();

            await _notificationPublisher.PublishAsync("Test", userIds: new[] { userIdentifier });

            var allNotifications = await
                _notificationStore.GetUserNotificationsWithNotificationsAsync(userIdentifier);

            allNotifications.Count.ShouldBe(1);
        }
        [Fact]
        public async Task Should_Get_All_Notifications_Between_StartDate_EndDate()
        {
            var userIdentifier = AbpSession.ToUserIdentifier();

            var now = DateTime.Now;
            await _notificationPublisher.PublishAsync("TestNotification", userIds: new[] { userIdentifier });

            Thread.Sleep(TimeSpan.FromSeconds(5));

            //this notification's creation time will be more than now+5sec
            await _notificationPublisher.PublishAsync("TestNotification2", userIds: new[] { userIdentifier });

            Thread.Sleep(TimeSpan.FromSeconds(5));

            //this notification's creation time will be more than now+10sec
            await _notificationPublisher.PublishAsync("TestNotification3", userIds: new[] { userIdentifier });


            //this should get second added notification
            var notifications = await _notificationStore.GetUserNotificationsWithNotificationsAsync(
                userIdentifier,
                startDate: now.AddSeconds(5),
                endDate: now.AddSeconds(10)
            );

            notifications.Count.ShouldBe(1);

            //this should get all added notification
            var allNotifications = await _notificationStore.GetUserNotificationsWithNotificationsAsync(
                userIdentifier,
                startDate: now,
                endDate: now.AddSeconds(30)
            );

            allNotifications.Count.ShouldBe(3);
        }
        [Fact]
        public async Task Should_Delete_All_Notifications_Between_StartDate_EndDate()
        {
            var now = DateTime.Now;

            var userIdentifier = AbpSession.ToUserIdentifier();
            await _notificationPublisher.PublishAsync("TestNotification", userIds: new[] { userIdentifier });

            Thread.Sleep(TimeSpan.FromSeconds(5));

            //this notification's creation time will be more than now+5sec
            await _notificationPublisher.PublishAsync("TestNotification2", userIds: new[] { userIdentifier });

            Thread.Sleep(TimeSpan.FromSeconds(5));

            //this notification's creation time will be more than now+10sec
            await _notificationPublisher.PublishAsync("TestNotification3", userIds: new[] { userIdentifier });

            var allNotifications = await _notificationStore.GetUserNotificationsWithNotificationsAsync(
                userIdentifier,
                startDate: now,
                endDate: now.AddSeconds(30)
            );

            allNotifications.Count.ShouldBe(3);

            //delete second added notification
            await _notificationStore.DeleteAllUserNotificationsAsync(
                userIdentifier,
                state: null,
                startDate: now.AddSeconds(5),
                endDate: now.AddSeconds(10)
            );

            //check
            var notifications = await _notificationStore.GetUserNotificationsWithNotificationsAsync(
                userIdentifier,
                startDate: now.AddSeconds(5),
                endDate: now.AddSeconds(10)
            );

            notifications.Count.ShouldBe(0);

            allNotifications = await _notificationStore.GetUserNotificationsWithNotificationsAsync(
                userIdentifier,
                startDate: now,
                endDate: now.AddSeconds(30)
            );

            allNotifications.Count.ShouldBe(2);
            
            //delete all added notification
            await _notificationStore.DeleteAllUserNotificationsAsync(
                userIdentifier,
                state: null,
                startDate: now,
                endDate: now.AddSeconds(30)
            );

            //check
            allNotifications = await _notificationStore.GetUserNotificationsWithNotificationsAsync(
                userIdentifier,
                startDate: now,
                endDate: now.AddSeconds(30)
            );

            allNotifications.Count.ShouldBe(0);
        }

    }
}
