using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Threading;

namespace Abp.Notifications
{
    /// <summary>
    /// This background job distributes notifications to users.
    /// </summary>
    public class NotificationDistributionJob : BackgroundJob<NotificationDistributionJobArgs>, ITransientDependency
    {
        private readonly INotificationConfiguration _notificationConfiguration;
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDistributionJob"/> class.
        /// </summary>
        public NotificationDistributionJob(
            INotificationConfiguration notificationConfiguration,
            IIocResolver iocResolver)
        {
            _notificationConfiguration = notificationConfiguration;
            _iocResolver = iocResolver;
        }

        public override void Execute(NotificationDistributionJobArgs args)
        {
            foreach (var notificationDistributorType in _notificationConfiguration.Distributers)
            {
                using (var notificationDistributer = _iocResolver.ResolveAsDisposable<INotificationDistributer>(notificationDistributorType))
                {
                    notificationDistributer.Object.Distribute(args.NotificationId);
                }
            }
        }
    }
}
