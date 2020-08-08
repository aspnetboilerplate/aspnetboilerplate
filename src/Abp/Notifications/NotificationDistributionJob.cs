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
        private readonly INotificationDistributer _notificationDistributer;
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDistributionJob"/> class.
        /// </summary>
        public NotificationDistributionJob(
            INotificationConfiguration notificationConfiguration,
            IIocResolver iocResolver, 
            INotificationDistributer notificationDistributer)
        {
            _notificationConfiguration = notificationConfiguration;
            _iocResolver = iocResolver;
            _notificationDistributer = notificationDistributer;
        }

        public override void Execute(NotificationDistributionJobArgs args)
        {
            _notificationDistributer.Distribute(args.NotificationId);
        }
    }
}
