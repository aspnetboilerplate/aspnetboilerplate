using Abp.Domain.Services;
using Taskever.Application.Services;

namespace Taskever.Domain.Services
{
    public interface INotificationService : IDomainService
    {
        void Notify(INotification notification);
    }
}
