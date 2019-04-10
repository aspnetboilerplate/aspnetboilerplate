using Abp.Collections;

namespace Abp.Notifications
{
    internal class NotificationConfiguration : INotificationConfiguration
    {
        public ITypeList<NotificationProvider> Providers { get; private set; }

        public ITypeList<INotificationDistributer> Distributers { get; private set; }

        public ITypeList<IRealTimeNotifier> Notifiers { get; private set; }

        public NotificationConfiguration()
        {
            Providers = new TypeList<NotificationProvider>();
            Distributers = new TypeList<INotificationDistributer>();
            Notifiers = new TypeList<IRealTimeNotifier>();
        }
    }
}