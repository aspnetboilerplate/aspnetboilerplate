using System;
using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;

namespace Abp.Notifications
{
    public class NotificationSubscriptionSynchronizer : IEventHandler<EntityDeletedEventData<AbpUserBase>>, ITransientDependency
    {
        private readonly IRepository<NotificationSubscriptionInfo, Guid> _notificationSubscriptionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public NotificationSubscriptionSynchronizer(
            IRepository<NotificationSubscriptionInfo, Guid> notificationSubscriptionRepository,
            IUnitOfWorkManager unitOfWorkManager
        )
        {
            _notificationSubscriptionRepository = notificationSubscriptionRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public virtual void HandleEvent(EntityDeletedEventData<AbpUserBase> eventData)
        {
            using (_unitOfWorkManager.Current.SetTenantId(eventData.Entity.TenantId))
            {
                _notificationSubscriptionRepository.Delete(x => x.UserId == eventData.Entity.Id);
            }
        }
    }
}
