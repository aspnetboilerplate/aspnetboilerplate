using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;

namespace Abp.Notifications
{
    /// <summary>
    /// This background job distributes notifications to users.
    /// </summary>
    public class NotificationDistributionJob : IAsyncBackgroundJob<NotificationDistributionJobArgs>, ITransientDependency
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

        public async Task ExecuteAsync(NotificationDistributionJobArgs args)
        {
            await _notificationDistributer.DistributeAsync(args.NotificationId);
        }
    }
}
