using System;
using System.Linq;
using System.Text;
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
        private readonly INotificationConfiguration _notificationConfiguration;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationPublisher"/> class.
        /// </summary>
        public NotificationPublisher(
            INotificationStore store,
            IBackgroundJobManager backgroundJobManager,
            INotificationDistributer notificationDistributer,
            IGuidGenerator guidGenerator, 
            INotificationConfiguration notificationConfiguration)
        {
            _store = store;
            _backgroundJobManager = backgroundJobManager;
            _notificationDistributer = notificationDistributer;
            _guidGenerator = guidGenerator;
            _notificationConfiguration = notificationConfiguration;
            AbpSession = NullAbpSession.Instance;
        }
        
        public virtual async Task PublishAsync(
            string notificationName,
            NotificationData data = null,
            EntityIdentifier entityIdentifier = null,
            NotificationSeverity severity = NotificationSeverity.Info,
            UserIdentifier[] userIds = null,
            UserIdentifier[] excludedUserIds = null,
            int?[] tenantIds = null,
            Type[] targetNotifiers = null)
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

                SetTargetNotifiers(notificationInfo, targetNotifiers);

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

        protected virtual void SetTargetNotifiers(NotificationInfo notificationInfo, Type[] targetNotifiers)
        {
            if (targetNotifiers == null)
            {
                return;
            }
            
            var allNotificationNotifiers = _notificationConfiguration.Notifiers.Select(notifier => notifier.FullName).ToList();
                    
            foreach (var targetNotifier in targetNotifiers)
            {
                if (!allNotificationNotifiers.Contains(targetNotifier.FullName))
                {
                    throw new ApplicationException("Given target notifier is not registered before: " + targetNotifier.FullName+" You must register it to the INotificationConfiguration.Notifiers!");
                }
            }

            notificationInfo.SetTargetNotifiers(targetNotifiers.Select(n => n.FullName).ToList());
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
