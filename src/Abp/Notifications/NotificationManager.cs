using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Collections.Extensions;
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
        private readonly IBackgroundJobManager _backgroundJobManager;

        public NotificationManager(
            INotificationStore store, 
            INotificationDefinitionManager notificationDefinitionManager,
            IUnitOfWorkManager unitOfWorkManager,
            IBackgroundJobManager backgroundJobManager)
        {
            _store = store;
            _notificationDefinitionManager = notificationDefinitionManager;
            _unitOfWorkManager = unitOfWorkManager;
            _backgroundJobManager = backgroundJobManager;

            Logger = NullLogger.Instance;
        }

        public async Task SubscribeAsync(NotificationSubscriptionOptions options)
        {
            CheckNotificationName(options.NotificationName);

            await _store.InsertSubscriptionAsync(options);
        }

        public async Task UnsubscribeAsync(NotificationSubscriptionOptions options)
        {
            CheckNotificationName(options.NotificationName);

            await _store.DeleteSubscriptionAsync(options);
        }

        [UnitOfWork]
        public virtual async Task PublishAsync(NotificationPublishOptions options)
        {
            CheckNotificationName(options.NotificationName);

            var notificationInfo = new NotificationInfo
            {
                NotificationName = options.NotificationName,
                EntityType = options.EntityType,
                EntityId = options.EntityId,
                Severity = options.Severity,
                UserIds = options.UserIds.IsNullOrEmpty() ? null : options.UserIds.JoinAsString(",")
            };

            await _store.InsertNotificationAsync(notificationInfo);
            
            await _backgroundJobManager.EnqueueAsync<NotificationDistributionJob, NotificationDistributionJobArgs>(
                new NotificationDistributionJobArgs(
                    notificationInfo.Id
                    )
                );
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