using Abp.Collections;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to configure notification system.
    /// </summary>
    public interface INotificationConfiguration
    {
        /// <summary>
        /// Notification providers.
        /// </summary>
        ITypeList<NotificationProvider> Providers { get; }

        /// <summary>
        /// A list of contributors for notification distribution process.
        /// </summary>
        ITypeList<INotificationDistributer> Distributers { get; }
    }
}