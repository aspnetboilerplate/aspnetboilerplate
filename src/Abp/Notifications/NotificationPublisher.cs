using System;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Json;
using Abp.Runtime.Session;

namespace Abp.Notifications
{
    /// <summary>
    /// Implements <see cref="INotificationPublisher"/>.
    /// </summary>
    public class NotificationPublisher : AbpServiceBase, INotificationPublisher, ITransientDependency
    {
        /// <summary>
        /// Indicates all tenants.
        /// </summary>
        public int[] AllTenants
        {
            get
            {
                return new[] { NotificationInfo.AllTenantIds.To<int>() };
            }
        }

        /// <summary>
        /// Reference to ABP session.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

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
            AbpSession = NullAbpSession.Instance;
        }

        //Create EntityIdentifier includes entityType and entityId.
        [UnitOfWork]
        public virtual async Task PublishAsync(
            string notificationName,
            NotificationData data = null,
            EntityIdentifier entityIdentifier = null,
            NotificationSeverity severity = NotificationSeverity.Info,
            long[] userIds = null,
            int[] tenantIds = null)
        {
            if (notificationName.IsNullOrEmpty())
            {
                throw new ArgumentException("NotificationName can not be null or whitespace!", "notificationName");
            }

            if (tenantIds.IsNullOrEmpty())
            {
                tenantIds = AbpSession.TenantId.HasValue
                    ? new[] { AbpSession.TenantId.Value }
                    : AllTenants;
            }
            else if (!userIds.IsNullOrEmpty())
            {
                throw new ArgumentException("tenantIds can be set only userIds is not set!", "tenantIds");
            }

            var notificationInfo = new NotificationInfo
            {
                NotificationName = notificationName,
                EntityTypeName = entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                EntityTypeAssemblyQualifiedName = entityIdentifier == null ? null : entityIdentifier.Type.AssemblyQualifiedName,
                EntityId = entityIdentifier == null ? null : entityIdentifier.Id.ToJsonString(),
                Severity = severity,
                UserIds = userIds.IsNullOrEmpty() ? null : userIds.JoinAsString(","),
                TenantIds = tenantIds.IsNullOrEmpty() ? null : tenantIds.JoinAsString(","),
                Data = data == null ? null : data.ToJsonString(),
                DataTypeName = data == null ? null : data.GetType().AssemblyQualifiedName
            };

            await _store.InsertNotificationAsync(notificationInfo);

            await CurrentUnitOfWork.SaveChangesAsync(); //To get Id of the notification

            await _backgroundJobManager.EnqueueAsync<NotificationDistributionJob, NotificationDistributionJobArgs>(
                new NotificationDistributionJobArgs(
                    notificationInfo.Id
                    )
                );
        }
    }
}