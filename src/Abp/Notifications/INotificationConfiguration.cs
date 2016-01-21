using Abp.Collections;

namespace Abp.Notifications
{
    public interface INotificationConfiguration
    {
        ITypeList<NotificationProvider> Providers { get; set; }
    }
}