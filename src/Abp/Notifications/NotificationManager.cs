using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Notifications
{
    public class NotificationManager : INotificationManager, ISingletonDependency
    {
        protected INotificationStore Store { get; private set; }

        public NotificationManager(INotificationStore store)
        {
            Store = store;
        }

        public async Task SubscribeAsync(NotificationSubscriptionOptions options)
        {
            await Store.InsertSubscriptionAsync(options);
        }

        public Task PublishAsync(NotificationPublishOptions options)
        {
            throw new System.NotImplementedException();
        }

        public Task SendAsync(NotificationSendOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}