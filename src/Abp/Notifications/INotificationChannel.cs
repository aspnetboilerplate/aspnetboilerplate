using System.Threading.Tasks;

namespace Abp.Notifications
{
    /// <summary>
    /// Additional notification channels, like SMS and EMAIL.
    /// </summary>
    public interface INotificationChannel
    {
        Task Send(Notification notification);
    }
}