using Abp.Collections;

namespace Abp.Notifications
{
    internal class NotificationConfiguration : INotificationConfiguration
    {
        public NotificationConfiguration()
        {
            Providers = new TypeList<NotificationProvider>();
        }

        public ITypeList<NotificationProvider> Providers { get; }
    }
}