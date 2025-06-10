using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Notifications;
using Abp.Runtime.Session;
using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace Abp.Zero.Notifications;

public class NotificationSubscription_Tests : AbpZeroTestBase
{
    private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
    private readonly Notifier1 _realTimeNotifier1;
    private readonly Notifier2 _realTimeNotifier2;
    private readonly INotificationPublisher _publisher;

    public NotificationSubscription_Tests()
    {
        _realTimeNotifier1 = new Notifier1();
        _realTimeNotifier2 = new Notifier2();

        RegisterRealTimeNotifiers(new List<IRealTimeNotifier>
            {
                _realTimeNotifier1,
                _realTimeNotifier2
            });

        AddRealTimeNotifiers(new List<IRealTimeNotifier>
            {
                _realTimeNotifier1,
                _realTimeNotifier2
            });

        _notificationSubscriptionManager = Resolve<INotificationSubscriptionManager>();
        _publisher = LocalIocManager.Resolve<INotificationPublisher>();
    }

    private void AddRealTimeNotifiers(List<IRealTimeNotifier> realTimeNotifiers)
    {
        // Use DefaultNotificationDistributer 
        var defaultNotificationDistributor = LocalIocManager.Resolve<DefaultNotificationDistributer>();
        LocalIocManager.IocContainer.Register(
            Component.For<INotificationDistributer>().Instance(defaultNotificationDistributor)
                .LifestyleSingleton()
                .IsDefault()
                .Named("DefaultNotificationDistributer")
        );

        // Add notifiers
        var notificationConfiguration = LocalIocManager.Resolve<INotificationConfiguration>();
        foreach (var realTimeNotifier in realTimeNotifiers)
        {
            notificationConfiguration.Notifiers.Add(realTimeNotifier.GetType());
        }
    }

    private void RegisterRealTimeNotifiers(List<IRealTimeNotifier> realTimeNotifiers)
    {
        foreach (var realTimeNotifier in realTimeNotifiers)
        {
            var realTimeNotifierType = realTimeNotifier.GetType();
            LocalIocManager.IocContainer.Register(
                Component.For(realTimeNotifierType)
                    .Instance(realTimeNotifier)
                    .LifestyleSingleton()
            );
        }
    }

    [Fact]
    public async Task Should_Not_Get_Not_Subscribed_Notification_Targets()
    {
        var notificationName = "CustomNotification";

        // Arrange -> subscribe to CustomNotification with _realTimeNotifier
        await _notificationSubscriptionManager.SubscribeAsync(
            AbpSession.ToUserIdentifier(),
            notificationName,
            null,
            _realTimeNotifier1.GetType().FullName
        );

        // Act
        var subscriptions = await _notificationSubscriptionManager.GetSubscriptionsAsync(
            AbpSession.TenantId,
            notificationName,
            null,
            _realTimeNotifier2.GetType().FullName
        );

        // Assert
        subscriptions.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_Not_Publish_Notification_For_Not_Subscribed_Notification_Targets()
    {
        var notificationName = "TestNotification";

        // Arrange -> subscribe to CustomNotification with _realTimeNotifier
        await _notificationSubscriptionManager.SubscribeAsync(
            AbpSession.ToUserIdentifier(),
            notificationName,
            null,
            _realTimeNotifier1.GetType().FullName
        );

        var notificationData = new NotificationData();

        //Act
        await _publisher.PublishAsync(
            "TestNotification",
            notificationData,
            severity: NotificationSeverity.Success,
            userIds: new[] { AbpSession.ToUserIdentifier() }
        );

        //Assert
        _realTimeNotifier1.IsSendNotificationCalled.ShouldBeTrue();
        _realTimeNotifier2.IsSendNotificationCalled.ShouldBeFalse();
    }
}