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

        public virtual void HandleEvent(EntityDeletedEventData<AbpUserBase> eventData)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    _notificationSubscriptionRepository.Delete(x => x.UserId == eventData.Entity.Id && x.TenantId == eventData.Entity.TenantId);
                    uow.CompleteAsync();
                }
            }
        }
    }
}