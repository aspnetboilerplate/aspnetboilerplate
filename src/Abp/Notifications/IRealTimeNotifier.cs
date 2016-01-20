using System.Threading.Tasks;

namespace Abp.Notifications
{
    public interface IRealTimeNotifier
    {
        Task SendNotificationAsync(long[] userIds, NotificationInfo notification);
    }
}