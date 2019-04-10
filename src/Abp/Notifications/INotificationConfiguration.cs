using Abp.Collections;
using System;

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
        [Obsolete]
        ITypeList<INotificationDistributer> Distributers { get; }

        /// <summary>
        /// A list of contributors for notification notifying process.
        /// </summary>
        ITypeList<IRealTimeNotifier> Notifiers { get; }
    }
}