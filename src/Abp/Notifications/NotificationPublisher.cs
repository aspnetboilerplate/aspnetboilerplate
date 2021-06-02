using System;
using System.Linq;
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
        public const int MaxUserCountToDirectlyDistributeANotification = 5;

        /// <summary>
        /// Indicates all tenants.
        /// </summary>
        public static int[] AllTenants => new[] { NotificationInfo.AllTenantIds.To<int>() };

        /// <summary>
        /// Reference to ABP session.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        private readonly INotificationStore _store;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly INotificationDistributer _notificationDistributer;
        private readonly IGuidGenerator _guidGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationPublisher"/> class.
        /// </summary>
        public NotificationPublisher(
            INotificationStore store,
            IBackgroundJobManager backgroundJobManager,
            INotificationDistributer notificationDistributer,
            IGuidGenerator guidGenerator)
        {
            _store = store;
            _backgroundJobManager = backgroundJobManager;
            _notificationDistributer = notificationDistributer;
            _guidGenerator = guidGenerator;
            AbpSession = NullAbpSession.Instance;
        }
        
        public virtual async Task PublishAsync(
            string notificationName,
            NotificationData data = null,
            EntityIdentifier entityIdentifier = null,
            NotificationSeverity severity = NotificationSeverity.Info,
            UserIdentifier[] userIds = null,
            UserIdentifier[] excludedUserIds = null,
            int?[] tenantIds = null)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                if (notificationName.IsNullOrEmpty())
                {
                    throw new ArgumentException("NotificationName can not be null or whitespace!", nameof(notificationName));
                }

                if (!tenantIds.IsNullOrEmpty() && !userIds.IsNullOrEmpty())
                {
                    throw new ArgumentException("tenantIds can be set only if userIds is not set!", nameof(tenantIds));
                }

                if (tenantIds.IsNullOrEmpty() && userIds.IsNullOrEmpty())
                {
                    tenantIds = new[] { AbpSession.TenantId };
                }

                var notificationInfo = new NotificationInfo(_guidGenerator.Create())
                {
                    NotificationName = notificationName,
                    EntityTypeName = entityIdentifier?.Type.FullName,
                    EntityTypeAssemblyQualifiedName = entityIdentifier?.Type.AssemblyQualifiedName,
                    EntityId = entityIdentifier?.Id.ToJsonString(),
                    Severity = severity,
                    UserIds = userIds.IsNullOrEmpty() ? null : userIds.Select(uid => uid.ToUserIdentifierString()).JoinAsString(","),
                    ExcludedUserIds = excludedUserIds.IsNullOrEmpty() ? null : excludedUserIds.Select(uid => uid.ToUserIdentifierString()).JoinAsString(","),
                    TenantIds = GetTenantIdsAsStr(tenantIds),
                    Data = data?.ToJsonString(),
                    DataTypeName = data?.GetType().AssemblyQualifiedName
                };

                await _store.InsertNotificationAsync(notificationInfo);

                await CurrentUnitOfWork.SaveChangesAsync(); //To get Id of the notification

                if (userIds != null && userIds.Length <= MaxUserCountToDirectlyDistributeANotification)
                {
                    //We can directly distribute the notification since there are not much receivers
                    await _notificationDistributer.DistributeAsync(notificationInfo.Id);
                }
                else
                {
                    //We enqueue a background job since distributing may get a long time
                    await _backgroundJobManager.EnqueueAsync<NotificationDistributionJob, NotificationDistributionJobArgs>(
                        new NotificationDistributionJobArgs(
                            notificationInfo.Id
                        )
                    );
                }
                
                await uow.CompleteAsync();
            }
        }

        /// <summary>
        /// Gets the string for <see cref="NotificationInfo.TenantIds"/>.
        /// </summary>
        /// <param name="tenantIds"></param>
        /// <seealso cref="DefaultNotificationDistributer.GetTenantIds"/>
        private static string GetTenantIdsAsStr(int?[] tenantIds)
        {
            if (tenantIds.IsNullOrEmpty())
            {
                return null;
            }

            return tenantIds
                .Select(tenantId => tenantId == null ? "null" : tenantId.ToString())
                .JoinAsString(",");
        }
    }
}
