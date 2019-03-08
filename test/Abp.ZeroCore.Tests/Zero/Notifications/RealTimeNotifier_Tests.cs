using System;
using System.Threading.Tasks;
using Abp.Notifications;
using Abp.Runtime.Session;
using Abp.Timing;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.Notifications
{
    public class RealTimeNotifier_Tests : AbpZeroTestBase
    {
        private readonly INotificationPublisher _publisher;
        private readonly IRealTimeNotifier _realTimeNotifier1;
        private readonly IRealTimeNotifier _realTimeNotifier2;

        public RealTimeNotifier_Tests()
        {
            _publisher = LocalIocManager.Resolve<INotificationPublisher>();
            _realTimeNotifier1 = Substitute.For<IRealTimeNotifier>();
            _realTimeNotifier2 = Substitute.For<IRealTimeNotifier2>();

            var realTimeNotifierType1 = _realTimeNotifier1.GetType();
            var realTimeNotifierType2 = _realTimeNotifier2.GetType();

            LocalIocManager.IocContainer.Register(
                Component.For(realTimeNotifierType1)
                    .Instance(_realTimeNotifier1)
                    .LifestyleSingleton()
            );
            LocalIocManager.IocContainer.Register(
                Component.For(realTimeNotifierType2)
                    .Instance(_realTimeNotifier2)
                    .LifestyleSingleton()
            );

            var notificationConfiguration = LocalIocManager.Resolve<INotificationConfiguration>();
            notificationConfiguration.Notifiers.Add(realTimeNotifierType1);
            notificationConfiguration.Notifiers.Add(realTimeNotifierType2);
        }

        [Fact]
        public async Task Should_Notify_Using_Custom_Notifiers()
        {
            //Arrange
            var notificationData = new NotificationData();

            //Act
            var before = Clock.Now;
            await _publisher.PublishAsync("TestNotification", notificationData, severity: NotificationSeverity.Success, userIds: new[] { AbpSession.ToUserIdentifier() });
            var after = Clock.Now;

            //Assert
            Predicate<UserNotification[]> predicate = userNotifications =>
            {
                userNotifications.Length.ShouldBe(1);

                var userNotification = userNotifications[0];
                userNotification.State.ShouldBe(UserNotificationState.Unread);
                userNotification.TenantId.ShouldBe(AbpSession.TenantId);
                userNotification.UserId.ShouldBe(AbpSession.UserId.Value);

                var notification = userNotification.Notification;
                notification.CreationTime.ShouldBeInRange(before, after);
                notification.Data.ToString().ShouldBe(notificationData.ToString());
                notification.EntityId.ShouldBe(null);
                notification.EntityType.ShouldBe(null);
                notification.EntityTypeName.ShouldBe(null);
                notification.NotificationName.ShouldBe("TestNotification");
                notification.Severity.ShouldBe(NotificationSeverity.Success);
                notification.TenantId.ShouldBe(AbpSession.TenantId);

                return true;
            };

            await _realTimeNotifier1.Received().SendNotificationsAsync(Arg.Is<UserNotification[]>(uns => predicate(uns)));
            await _realTimeNotifier2.Received().SendNotificationsAsync(Arg.Is<UserNotification[]>(uns => predicate(uns)));
        }
    }
}
