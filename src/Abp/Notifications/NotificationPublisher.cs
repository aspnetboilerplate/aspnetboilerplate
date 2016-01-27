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
    /// Implements <see cref="INotificationPublisher"/>.
    /// </summary>
    public class NotificationPublisher : INotificationPublisher, ITransientDependency
    {
        private readonly INotificationStore _store;
        private readonly IBackgroundJobManager _backgroundJobManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationPublisher"/> class.
        /// </summary>
        public NotificationPublisher(
            INotificationStore store,
            IBackgroundJobManager backgroundJobManager)
        {
            _store = store;
            _backgroundJobManager = backgroundJobManager;
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