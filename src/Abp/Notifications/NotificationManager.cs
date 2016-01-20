using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Castle.Core.Logging;

namespace Abp.Notifications
{
    public class NotificationManager : INotificationManager, ISingletonDependency
    {
        public ILogger Logger { get; set; }

        private readonly INotificationStore _store;
        private readonly INotificationDefinitionManager _notificationDefinitionManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUserNotificationQueue _userNotificationQueue;

        public NotificationManager(
            INotificationStore store, 
            INotificationDefinitionManager notificationDefinitionManager,
            IUnitOfWorkManager unitOfWorkManager,
            IUserNotificationQueue userNotificationQueue)
        {
            _store = store;
            _notificationDefinitionManager = notificationDefinitionManager;
            _unitOfWorkManager = unitOfWorkManager;
            _userNotificationQueue = userNotificationQueue;

            Logger = NullLogger.Instance;
        }

        public async Task SubscribeAsync(NotificationSubscriptionOptions options)
        {
            CheckNotificationName(options.NotificationName);

            await _store.InsertSubscriptionAsync(options);
        }

        public virtual async Task PublishAsync(NotificationPublishOptions options)
        {
            CheckNotificationName(options.Notification.NotificationName);

            await _store.InsertNotificationAsync(options.Notification);
            _userNotificationQueue.Add(options.Notification);
        }

        private void CheckNotificationName(string notificationName)
        {
            if (_notificationDefinitionManager.GetOrNull(notificationName) == null)
            {
                throw new AbpException(string.Format("There is no defined notificationName with {0}", notificationName));
            }
        }
    }
}