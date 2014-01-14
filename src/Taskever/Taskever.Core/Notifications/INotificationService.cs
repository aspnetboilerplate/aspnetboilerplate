using Abp.Domain.Services;

namespace Taskever.Notifications
{
    public interface INotificationService : IDomainService
    {
        void Notify(INotification notification);
    }
}
