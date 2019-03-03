using System.Threading.Tasks;
using Abp.Notifications;
using Abp.Runtime.Session;
using Shouldly;
using Xunit;

namespace Abp.Zero.Notifications
{
    public class RealTimeNotifier_Tests : AbpZeroTestBase
    {
        private readonly INotificationPublisher _publisher;
        private readonly FakeRealTimeNotifier1 _fakeRealTimeNotifier1;
        private readonly FakeRealTimeNotifier2 _fakeRealTimeNotifier2;

        public RealTimeNotifier_Tests()
        {
            _publisher = LocalIocManager.Resolve<INotificationPublisher>();
            _fakeRealTimeNotifier1 = LocalIocManager.Resolve<FakeRealTimeNotifier1>();
            _fakeRealTimeNotifier2 = LocalIocManager.Resolve<FakeRealTimeNotifier2>();
        }

        [Fact]
        public async Task Should_Notify_Using_Custom_Notifiers()
        {
            //Arrange
            var notificationData = new NotificationData();

            //Act
            await _publisher.PublishAsync("TestNotification", notificationData, severity: NotificationSeverity.Success, userIds: new[] { AbpSession.ToUserIdentifier() });

            //Assert
            _fakeRealTimeNotifier1.IsSendCalled.ShouldBeTrue();
            _fakeRealTimeNotifier2.IsSendCalled.ShouldBeTrue();
        }
    }
}
