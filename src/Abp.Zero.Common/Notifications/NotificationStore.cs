using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Abp.Linq.Extensions;

namespace Abp.Notifications
{
    /// <summary>
    /// Implements <see cref="INotificationStore"/> using repositories.
    /// </summary>
    public class NotificationStore : INotificationStore, ITransientDependency
    {
        private readonly IRepository<NotificationInfo, Guid> _notificationRepository;
        private readonly IRepository<TenantNotificationInfo, Guid> _tenantNotificationRepository;
        private readonly IRepository<UserNotificationInfo, Guid> _userNotificationRepository;
        private readonly IRepository<NotificationSubscriptionInfo, Guid> _notificationSubscriptionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationStore"/> class.
        /// </summary>
        public NotificationStore(
            IRepository<NotificationInfo, Guid> notificationRepository,
            IRepository<TenantNotificationInfo, Guid> tenantNotificationRepository,
            IRepository<UserNotificationInfo, Guid> userNotificationRepository,
            IRepository<NotificationSubscriptionInfo, Guid> notificationSubscriptionRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _notificationRepository = notificationRepository;
            _tenantNotificationRepository = tenantNotificationRepository;
            _userNotificationRepository = userNotificationRepository;
            _notificationSubscriptionRepository = notificationSubscriptionRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public virtual async Task InsertSubscriptionAsync(NotificationSubscriptionInfo subscription)
        {
            using (_unitOfWorkManager.Current.SetTenantId(subscription.TenantId))
            {
                await _notificationSubscriptionRepository.InsertAsync(subscription);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual void InsertSubscription(NotificationSubscriptionInfo subscription)
        {
            using (_unitOfWorkManager.Current.SetTenantId(subscription.TenantId))
            {
                _notificationSubscriptionRepository.Insert(subscription);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        [UnitOfWork]
        public virtual async Task DeleteSubscriptionAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                await _notificationSubscriptionRepository.DeleteAsync(s =>
                    s.UserId == user.UserId &&
                    s.NotificationName == notificationName &&
                    s.EntityTypeName == entityTypeName &&
                    s.EntityId == entityId
                    );
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual void DeleteSubscription(UserIdentifier user, string notificationName, string entityTypeName, string entityId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                _notificationSubscriptionRepository.Delete(s =>
                    s.UserId == user.UserId &&
                    s.NotificationName == notificationName &&
                    s.EntityTypeName == entityTypeName &&
                    s.EntityId == entityId
                    );
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        [UnitOfWork]
        public virtual async Task InsertNotificationAsync(NotificationInfo notification)
        {
            using (_unitOfWorkManager.Current.SetTenantId(null))
            {
                await _notificationRepository.InsertAsync(notification);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual void InsertNotification(NotificationInfo notification)
        {
            using (_unitOfWorkManager.Current.SetTenantId(null))
            {
                _notificationRepository.Insert(notification);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        [UnitOfWork]
        public virtual async Task<NotificationInfo> GetNotificationOrNullAsync(Guid notificationId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(null))
            {
                return await _notificationRepository.FirstOrDefaultAsync(notificationId);
            }
        }

        [UnitOfWork]
        public virtual NotificationInfo GetNotificationOrNull(Guid notificationId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(null))
            {
                return _notificationRepository.FirstOrDefault(notificationId);
            }
        }

        [UnitOfWork]
        public virtual async Task InsertUserNotificationAsync(UserNotificationInfo userNotification)
        {
            using (_unitOfWorkManager.Current.SetTenantId(userNotification.TenantId))
            {
                await _userNotificationRepository.InsertAsync(userNotification);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual void InsertUserNotification(UserNotificationInfo userNotification)
        {
            using (_unitOfWorkManager.Current.SetTenantId(userNotification.TenantId))
            {
                _userNotificationRepository.Insert(userNotification);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        [UnitOfWork]
        public virtual Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(string notificationName, string entityTypeName, string entityId)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                return _notificationSubscriptionRepository.GetAllListAsync(s =>
                    s.NotificationName == notificationName &&
                    s.EntityTypeName == entityTypeName &&
                    s.EntityId == entityId
                    );
            }
        }

        [UnitOfWork]
        public virtual List<NotificationSubscriptionInfo> GetSubscriptions(string notificationName, string entityTypeName, string entityId)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                return _notificationSubscriptionRepository.GetAllList(s =>
                    s.NotificationName == notificationName &&
                    s.EntityTypeName == entityTypeName &&
                    s.EntityId == entityId
                    );
            }
        }

        [UnitOfWork]
        public virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(int?[] tenantIds, string notificationName, string entityTypeName, string entityId)
        {
            var subscriptions = new List<NotificationSubscriptionInfo>();

            foreach (var tenantId in tenantIds)
            {
                subscriptions.AddRange(await GetSubscriptionsAsync(tenantId, notificationName, entityTypeName, entityId));
            }

            return subscriptions;
        }

        [UnitOfWork]
        public virtual List<NotificationSubscriptionInfo> GetSubscriptions(int?[] tenantIds, string notificationName, string entityTypeName, string entityId)
        {
            var subscriptions = new List<NotificationSubscriptionInfo>();

            foreach (var tenantId in tenantIds)
            {
                subscriptions.AddRange(GetSubscriptions(tenantId, notificationName, entityTypeName, entityId));
            }

            return subscriptions;
        }

        [UnitOfWork]
        public virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(UserIdentifier user)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                return await _notificationSubscriptionRepository.GetAllListAsync(s => s.UserId == user.UserId);
            }
        }

        [UnitOfWork]
        public virtual List<NotificationSubscriptionInfo> GetSubscriptions(UserIdentifier user)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                return _notificationSubscriptionRepository.GetAllList(s => s.UserId == user.UserId);
            }
        }

        [UnitOfWork]
        protected virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(int? tenantId, string notificationName, string entityTypeName, string entityId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return await _notificationSubscriptionRepository.GetAllListAsync(s =>
                    s.NotificationName == notificationName &&
                    s.EntityTypeName == entityTypeName &&
                    s.EntityId == entityId
                );
            }
        }

        [UnitOfWork]
        protected virtual List<NotificationSubscriptionInfo> GetSubscriptions(int? tenantId, string notificationName, string entityTypeName, string entityId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _notificationSubscriptionRepository.GetAllList(s =>
                    s.NotificationName == notificationName &&
                    s.EntityTypeName == entityTypeName &&
                    s.EntityId == entityId
                );
            }
        }

        [UnitOfWork]
        public virtual async Task<bool> IsSubscribedAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                return await _notificationSubscriptionRepository.CountAsync(s =>
                    s.UserId == user.UserId &&
                    s.NotificationName == notificationName &&
                    s.EntityTypeName == entityTypeName &&
                    s.EntityId == entityId
                    ) > 0;
            }
        }

        [UnitOfWork]
        public virtual bool IsSubscribed(UserIdentifier user, string notificationName, string entityTypeName, string entityId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                return _notificationSubscriptionRepository.Count(s =>
                    s.UserId == user.UserId &&
                    s.NotificationName == notificationName &&
                    s.EntityTypeName == entityTypeName &&
                    s.EntityId == entityId
                    ) > 0;
            }
        }

        [UnitOfWork]
        public virtual async Task UpdateUserNotificationStateAsync(int? tenantId, Guid userNotificationId, UserNotificationState state)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var userNotification = await _userNotificationRepository.FirstOrDefaultAsync(userNotificationId);
                if (userNotification == null)
                {
                    return;
                }

                userNotification.State = state;
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual void UpdateUserNotificationState(int? tenantId, Guid userNotificationId, UserNotificationState state)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var userNotification = _userNotificationRepository.FirstOrDefault(userNotificationId);
                if (userNotification == null)
                {
                    return;
                }

                userNotification.State = state;
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        [UnitOfWork]
        public virtual async Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var userNotifications = await _userNotificationRepository.GetAllListAsync(un => un.UserId == user.UserId);

                foreach (var userNotification in userNotifications)
                {
                    userNotification.State = state;
                }

                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual void UpdateAllUserNotificationStates(UserIdentifier user, UserNotificationState state)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var userNotifications = _userNotificationRepository.GetAllList(un => un.UserId == user.UserId);

                foreach (var userNotification in userNotifications)
                {
                    userNotification.State = state;
                }

                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        [UnitOfWork]
        public virtual async Task DeleteUserNotificationAsync(int? tenantId, Guid userNotificationId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                await _userNotificationRepository.DeleteAsync(userNotificationId);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual void DeleteUserNotification(int? tenantId, Guid userNotificationId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                _userNotificationRepository.Delete(userNotificationId);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        [UnitOfWork]
        public virtual async Task DeleteAllUserNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);

                await _userNotificationRepository.DeleteAsync(predicate);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        
        [UnitOfWork]
        public virtual void DeleteAllUserNotifications(UserIdentifier user, 
            UserNotificationState? state = null,
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);

                _userNotificationRepository.Delete(predicate);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        private Expression<Func<UserNotificationInfo, bool>> CreateNotificationFilterPredicate(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var predicate = PredicateBuilder.New<UserNotificationInfo>();
            predicate = predicate.And(p => p.UserId == user.UserId);

            if (startDate.HasValue)
            {
                predicate = predicate.And(p => p.CreationTime >= startDate);
            }

            if (endDate.HasValue)
            {
                predicate = predicate.And(p => p.CreationTime <= endDate);
            }

            if (state.HasValue)
            {
                predicate = predicate.And(p => p.State == state);
            }

            return predicate;
        }

        [UnitOfWork]
        public virtual Task<List<UserNotificationInfoWithNotificationInfo>> GetUserNotificationsWithNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var query = from userNotificationInfo in _userNotificationRepository.GetAll()
                            join tenantNotificationInfo in _tenantNotificationRepository.GetAll() on userNotificationInfo.TenantNotificationId equals tenantNotificationInfo.Id
                            where userNotificationInfo.UserId == user.UserId
                            orderby tenantNotificationInfo.CreationTime descending
                            select new { userNotificationInfo, tenantNotificationInfo = tenantNotificationInfo };

                if (state.HasValue)
                {
                    query = query.Where(x => x.userNotificationInfo.State == state.Value);
                }

                if (startDate.HasValue)
                {
                    query = query.Where(x => x.tenantNotificationInfo.CreationTime >= startDate);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(x => x.tenantNotificationInfo.CreationTime <= endDate);
                }

                query = query.PageBy(skipCount, maxResultCount);

                var list = query.ToList();

                return Task.FromResult(list.Select(
                    a => new UserNotificationInfoWithNotificationInfo(a.userNotificationInfo, a.tenantNotificationInfo)
                ).ToList());
            }
        }

        [UnitOfWork]
        public virtual List<UserNotificationInfoWithNotificationInfo> GetUserNotificationsWithNotifications(UserIdentifier user, UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var query = from userNotificationInfo in _userNotificationRepository.GetAll()
                    join tenantNotificationInfo in _tenantNotificationRepository.GetAll() on userNotificationInfo.TenantNotificationId equals tenantNotificationInfo.Id
                    where userNotificationInfo.UserId == user.UserId
                    orderby tenantNotificationInfo.CreationTime descending
                    select new { userNotificationInfo, tenantNotificationInfo = tenantNotificationInfo };

                if (state.HasValue)
                {
                    query = query.Where(x => x.userNotificationInfo.State == state.Value);
                }

                if (startDate.HasValue)
                {
                    query = query.Where(x => x.tenantNotificationInfo.CreationTime >= startDate);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(x => x.tenantNotificationInfo.CreationTime <= endDate);
                }

                query = query.PageBy(skipCount, maxResultCount);

                var list = query.ToList();

                return list.Select(
                    a => new UserNotificationInfoWithNotificationInfo(a.userNotificationInfo, a.tenantNotificationInfo)
                ).ToList();
            }
        }

        [UnitOfWork]
        public virtual async Task<int> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);
                return await _userNotificationRepository.CountAsync(predicate);
            }
        }

        [UnitOfWork]
        public virtual int GetUserNotificationCount(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);
                return _userNotificationRepository.Count(predicate);
            }
        }

        [UnitOfWork]
        public virtual Task<UserNotificationInfoWithNotificationInfo> GetUserNotificationWithNotificationOrNullAsync(int? tenantId, Guid userNotificationId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var query = from userNotificationInfo in _userNotificationRepository.GetAll()
                            join tenantNotificationInfo in _tenantNotificationRepository.GetAll() on userNotificationInfo.TenantNotificationId equals tenantNotificationInfo.Id
                            where userNotificationInfo.Id == userNotificationId
                            select new { userNotificationInfo, tenantNotificationInfo = tenantNotificationInfo };

                var item = query.FirstOrDefault();
                if (item == null)
                {
                    return Task.FromResult((UserNotificationInfoWithNotificationInfo)null);
                }

                return Task.FromResult(new UserNotificationInfoWithNotificationInfo(item.userNotificationInfo, item.tenantNotificationInfo));
            }
        }

        [UnitOfWork]
        public virtual UserNotificationInfoWithNotificationInfo GetUserNotificationWithNotificationOrNull(int? tenantId, Guid userNotificationId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var query = from userNotificationInfo in _userNotificationRepository.GetAll()
                            join tenantNotificationInfo in _tenantNotificationRepository.GetAll() on userNotificationInfo.TenantNotificationId equals tenantNotificationInfo.Id
                            where userNotificationInfo.Id == userNotificationId
                            select new { userNotificationInfo, tenantNotificationInfo = tenantNotificationInfo };

                var item = query.FirstOrDefault();
                if (item == null)
                {
                    return (UserNotificationInfoWithNotificationInfo)null;
                }

                return new UserNotificationInfoWithNotificationInfo(item.userNotificationInfo, item.tenantNotificationInfo);
            }
        }

        [UnitOfWork]
        public virtual async Task InsertTenantNotificationAsync(TenantNotificationInfo tenantNotificationInfo)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantNotificationInfo.TenantId))
            {
                await _tenantNotificationRepository.InsertAsync(tenantNotificationInfo);
            }
        }

        [UnitOfWork]
        public virtual void InsertTenantNotification(TenantNotificationInfo tenantNotificationInfo)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantNotificationInfo.TenantId))
            {
                _tenantNotificationRepository.Insert(tenantNotificationInfo);
            }
        }

        public virtual Task DeleteNotificationAsync(NotificationInfo notification)
        {
            return _notificationRepository.DeleteAsync(notification);
        }

        public virtual void DeleteNotification(NotificationInfo notification)
        {
            _notificationRepository.Delete(notification);
        }
    }
}
