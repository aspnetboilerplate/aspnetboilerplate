using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Json;
using Castle.Core.Logging;

namespace Abp.Notifications
{
    /// <summary>
    /// Implements <see cref="INotificationManager"/>.
    /// </summary>
    public class NotificationManager : INotificationManager, ISingletonDependency
    {
        public ILogger Logger { get; set; }

        private readonly INotificationStore _store;
        private readonly IBackgroundJobManager _backgroundJobManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationManager"/> class.
        /// </summary>
        public NotificationManager(
            INotificationStore store,
            IBackgroundJobManager backgroundJobManager)
        {
            _store = store;
            _backgroundJobManager = backgroundJobManager;

            Logger = NullLogger.Instance;
        }

        public async Task SubscribeAsync(NotificationSubscriptionOptions options)
        {
            var subscriptionInfo = new NotificationSubscriptionInfo
            {
                NotificationName = options.NotificationName,
                UserId = options.UserId,
                EntityTypeName = options.EntityType.FullName,
                EntityId = options.EntityId.ToString(), //TODO: ToString() can be problem for some types, use JSON serialization instead, based on entity's primary key type
            };

            await _store.InsertSubscriptionAsync(subscriptionInfo);
        }

        public async Task UnsubscribeAsync(NotificationSubscriptionOptions options)
        {
            var subscriptionInfo = new NotificationSubscriptionInfo
            {
                NotificationName = options.NotificationName,
                UserId = options.UserId,
                EntityTypeName = options.EntityType.FullName,
                EntityId = options.EntityId.ToString(), //TODO: ToString() can be problem for some types, use JSON serialization instead, based on entity's primary key type
            };

            await _store.DeleteSubscriptionAsync(subscriptionInfo);
        }

        [UnitOfWork]
        public virtual async Task PublishAsync(NotificationPublishOptions options)
        {
            var notificationInfo = new NotificationInfo
            {
                NotificationName = options.NotificationName,
                EntityTypeName = options.EntityType == null ? null : options.EntityType.FullName,
                EntityTypeAssemblyQualifiedName = options.EntityType == null ? null : options.EntityType.AssemblyQualifiedName,
                EntityId = options.EntityId == null ? null : options.EntityId.ToJsonString(),
                Severity = options.Severity,
                UserIds = options.UserIds.IsNullOrEmpty() ? null : options.UserIds.JoinAsString(","),
                Data = options.Data.ToJsonString(),
                DataTypeName = options.Data.GetType().AssemblyQualifiedName
            };

            await _store.InsertNotificationAsync(notificationInfo);
            
            await _backgroundJobManager.EnqueueAsync<NotificationDistributionJob, NotificationDistributionJobArgs>(
                new NotificationDistributionJobArgs(
                    notificationInfo.Id
                    )
                );
        }
    }
}