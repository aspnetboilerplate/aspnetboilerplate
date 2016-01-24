using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Notifications
{
    public interface IRealTimeNotifier
    {
        Task SendNotificationAsync(NotificationInfo notification, IEnumerable<UserNotificationInfo> userNotifications);
    }
}