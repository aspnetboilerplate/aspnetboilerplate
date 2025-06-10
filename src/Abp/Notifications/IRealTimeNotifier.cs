using System.Threading.Tasks;
using Abp.Localization;

namespace Abp.Notifications
{
    /// <summary>
    /// Interface to send real time notifications to users.
    /// </summary>
    public interface IRealTimeNotifier
    {
        /// <summary>
        /// This method tries to deliver real time notifications to specified users.
        /// If a user is not online, it should ignore him.
        /// </summary>
        Task SendNotificationsAsync(UserNotification[] userNotifications);
        
        /// <summary>
        /// If true, this real time notifier will be used for sending real time notifications when it is requested. Otherwise it will not be used.
        /// <para>
        /// If false, this realtime notifier will notify any notifications.
        /// </para>
        /// </summary>
        bool UseOnlyIfRequestedAsTarget { get; }
    }
}
