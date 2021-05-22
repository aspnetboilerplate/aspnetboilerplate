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

        public virtual async Task InsertSubscriptionAsync(NotificationSubscriptionInfo subscription)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(subscription.TenantId))
                {
                    await _notificationSubscriptionRepository.InsertAsync(subscription);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        public virtual void InsertSubscription(NotificationSubscriptionInfo subscription)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(subscription.TenantId))
                {
                    _notificationSubscriptionRepository.Insert(subscription);
                    _unitOfWorkManager.Current.SaveChanges();
                }

                uow.Complete();
            }
        }

        public virtual async Task DeleteSubscriptionAsync(UserIdentifier user, string notificationName,
            string entityTypeName, string entityId)
        {
            using (var uow = _unitOfWorkManager.Begin())
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

                await uow.CompleteAsync();
            }
        }

        public virtual void DeleteSubscription(UserIdentifier user, string notificationName, string entityTypeName,
            string entityId)
        {
            using (var uow = _unitOfWorkManager.Begin())
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

                uow.Complete();
            }
        }

        public virtual async Task InsertNotificationAsync(NotificationInfo notification)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    await _notificationRepository.InsertAsync(notification);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        public virtual void InsertNotification(NotificationInfo notification)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    _notificationRepository.Insert(notification);
                    _unitOfWorkManager.Current.SaveChanges();
                }

                uow.Complete();
            }
        }

        public virtual async Task<NotificationInfo> GetNotificationOrNullAsync(Guid notificationId)
        {
            NotificationInfo notification;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    notification = await _notificationRepository.FirstOrDefaultAsync(notificationId);
                }

                await uow.CompleteAsync();
            }

            return notification;
        }

        public virtual NotificationInfo GetNotificationOrNull(Guid notificationId)
        {
            NotificationInfo notification;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    notification = _notificationRepository.FirstOrDefault(notificationId);
                }

                uow.Complete();
            }

            return notification;
        }

        public virtual async Task InsertUserNotificationAsync(UserNotificationInfo userNotification)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(userNotification.TenantId))
                {
                    await _userNotificationRepository.InsertAsync(userNotification);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        public virtual void InsertUserNotification(UserNotificationInfo userNotification)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(userNotification.TenantId))
                {
                    _userNotificationRepository.Insert(userNotification);
                    _unitOfWorkManager.Current.SaveChanges();
                }

                uow.Complete();
            }
        }

        public virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            List<NotificationSubscriptionInfo> notificationSubscriptions;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    notificationSubscriptions = await _notificationSubscriptionRepository.GetAllListAsync(s =>
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    );
                }

                await uow.CompleteAsync();
            }

            return notificationSubscriptions;
        }

        public virtual List<NotificationSubscriptionInfo> GetSubscriptions(
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            List<NotificationSubscriptionInfo> notificationSubscriptions;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    notificationSubscriptions = _notificationSubscriptionRepository.GetAllList(s =>
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    );
                }

                uow.Complete();
            }

            return notificationSubscriptions;
        }

        public virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(
            int?[] tenantIds,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            var subscriptions = new List<NotificationSubscriptionInfo>();

            using (var uow = _unitOfWorkManager.Begin())
            {
                foreach (var tenantId in tenantIds)
                {
                    subscriptions.AddRange(
                        await GetSubscriptionsAsync(tenantId, notificationName, entityTypeName, entityId));
                }

                await uow.CompleteAsync();
            }

            return subscriptions;
        }

        public virtual List<NotificationSubscriptionInfo> GetSubscriptions(
            int?[] tenantIds,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            var subscriptions = new List<NotificationSubscriptionInfo>();

            using (var uow = _unitOfWorkManager.Begin())
            {
                foreach (var tenantId in tenantIds)
                {
                    subscriptions.AddRange(GetSubscriptions(tenantId, notificationName, entityTypeName, entityId));
                }

                uow.Complete();
            }

            return subscriptions;
        }

        public virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(UserIdentifier user)
        {
            List<NotificationSubscriptionInfo> notificationSubscriptions;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    notificationSubscriptions =
                        await _notificationSubscriptionRepository.GetAllListAsync(s => s.UserId == user.UserId);
                }

                await uow.CompleteAsync();
            }

            return notificationSubscriptions;
        }

        public virtual List<NotificationSubscriptionInfo> GetSubscriptions(UserIdentifier user)
        {
            List<NotificationSubscriptionInfo> notificationSubscriptions;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    notificationSubscriptions =
                        _notificationSubscriptionRepository.GetAllList(s => s.UserId == user.UserId);
                }

                uow.Complete();
            }

            return notificationSubscriptions;
        }

        protected virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(
            int? tenantId,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            List<NotificationSubscriptionInfo> notificationSubscriptions;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    notificationSubscriptions = await _notificationSubscriptionRepository.GetAllListAsync(s =>
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    );
                }

                await uow.CompleteAsync();
            }

            return notificationSubscriptions;
        }

        protected virtual List<NotificationSubscriptionInfo> GetSubscriptions(
            int? tenantId,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            List<NotificationSubscriptionInfo> notificationSubscriptions;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    notificationSubscriptions = _notificationSubscriptionRepository.GetAllList(s =>
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    );
                }

                uow.Complete();
            }

            return notificationSubscriptions;
        }

        public virtual async Task<bool> IsSubscribedAsync(
            UserIdentifier user,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            bool subscribed;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    subscribed = await _notificationSubscriptionRepository.CountAsync(s =>
                        s.UserId == user.UserId &&
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    ) > 0;
                }

                await uow.CompleteAsync();
            }

            return subscribed;
        }

        public virtual bool IsSubscribed(
            UserIdentifier user,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            bool subscribed;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    subscribed = _notificationSubscriptionRepository.Count(s =>
                        s.UserId == user.UserId &&
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    ) > 0;
                }

                uow.Complete();
            }

            return subscribed;
        }

        public virtual async Task UpdateUserNotificationStateAsync(
            int? tenantId,
            Guid userNotificationId,
            UserNotificationState state)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var userNotification = await _userNotificationRepository.FirstOrDefaultAsync(userNotificationId);
                    if (userNotification == null)
                    {
                        await uow.CompleteAsync();
                        return;
                    }

                    userNotification.State = state;
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        public virtual void UpdateUserNotificationState(
            int? tenantId,
            Guid userNotificationId,
            UserNotificationState state)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var userNotification = _userNotificationRepository.FirstOrDefault(userNotificationId);
                    if (userNotification == null)
                    {
                        uow.Complete();
                        return;
                    }

                    userNotification.State = state;
                    _unitOfWorkManager.Current.SaveChanges();
                }

                uow.Complete();
            }
        }

        public virtual async Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var userNotifications = await _userNotificationRepository.GetAllListAsync(
                        un => un.UserId == user.UserId
                    );

                    foreach (var userNotification in userNotifications)
                    {
                        userNotification.State = state;
                    }

                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        public virtual void UpdateAllUserNotificationStates(UserIdentifier user, UserNotificationState state)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var userNotifications = _userNotificationRepository.GetAllList(
                        un => un.UserId == user.UserId
                    );

                    foreach (var userNotification in userNotifications)
                    {
                        userNotification.State = state;
                    }

                    _unitOfWorkManager.Current.SaveChanges();
                }

                uow.Complete();
            }
        }

        public virtual async Task DeleteUserNotificationAsync(int? tenantId, Guid userNotificationId)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await _userNotificationRepository.DeleteAsync(userNotificationId);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        public virtual void DeleteUserNotification(int? tenantId, Guid userNotificationId)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    _userNotificationRepository.Delete(userNotificationId);
                    _unitOfWorkManager.Current.SaveChanges();
                }

                uow.Complete();
            }
        }

        public virtual async Task DeleteAllUserNotificationsAsync(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);

                    await _userNotificationRepository.DeleteAsync(predicate);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        public virtual void DeleteAllUserNotifications(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);

                    _userNotificationRepository.Delete(predicate);
                    _unitOfWorkManager.Current.SaveChanges();
                }

                uow.Complete();
            }
        }

        private Expression<Func<UserNotificationInfo, bool>> CreateNotificationFilterPredicate(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
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

        public virtual async Task<List<UserNotificationInfoWithNotificationInfo>>
            GetUserNotificationsWithNotificationsAsync(
                UserIdentifier user,
                UserNotificationState? state = null,
                int skipCount = 0,
                int maxResultCount = int.MaxValue,
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            List<UserNotificationInfoWithNotificationInfo> result;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var query = from userNotificationInfo in _userNotificationRepository.GetAll()
                        join tenantNotificationInfo in _tenantNotificationRepository.GetAll() on userNotificationInfo
                            .TenantNotificationId equals tenantNotificationInfo.Id
                        where userNotificationInfo.UserId == user.UserId
                        orderby tenantNotificationInfo.CreationTime descending
                        select new
                        {
                            userNotificationInfo,
                            tenantNotificationInfo
                        };

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

                    result = list.Select(
                        a => new UserNotificationInfoWithNotificationInfo(
                            a.userNotificationInfo,
                            a.tenantNotificationInfo
                        )
                    ).ToList();
                }

                await uow.CompleteAsync();
            }

            return result;
        }

        public virtual List<UserNotificationInfoWithNotificationInfo> GetUserNotificationsWithNotifications(
            UserIdentifier user,
            UserNotificationState? state = null,
            int skipCount = 0,
            int maxResultCount = int.MaxValue,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            List<UserNotificationInfoWithNotificationInfo> result;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var query = from userNotificationInfo in _userNotificationRepository.GetAll()
                        join tenantNotificationInfo in _tenantNotificationRepository.GetAll() on userNotificationInfo
                            .TenantNotificationId equals tenantNotificationInfo.Id
                        where userNotificationInfo.UserId == user.UserId
                        orderby tenantNotificationInfo.CreationTime descending
                        select new
                        {
                            userNotificationInfo,
                            tenantNotificationInfo
                        };

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

                    result = list.Select(
                        a => new UserNotificationInfoWithNotificationInfo(
                            a.userNotificationInfo,
                            a.tenantNotificationInfo
                        )
                    ).ToList();
                }

                uow.Complete();
            }

            return result;
        }

        public virtual async Task<int> GetUserNotificationCountAsync(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            int count;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);
                    count = await _userNotificationRepository.CountAsync(predicate);
                }

                await uow.CompleteAsync();
            }

            return count;
        }

        public virtual int GetUserNotificationCount(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            int count;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);
                    count = _userNotificationRepository.Count(predicate);
                }

                uow.CompleteAsync();
            }

            return count;
        }

        public virtual async Task<UserNotificationInfoWithNotificationInfo>
            GetUserNotificationWithNotificationOrNullAsync(
                int? tenantId,
                Guid userNotificationId)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var query = from userNotificationInfo in _userNotificationRepository.GetAll()
                        join tenantNotificationInfo in _tenantNotificationRepository.GetAll() on userNotificationInfo
                            .TenantNotificationId equals tenantNotificationInfo.Id
                        where userNotificationInfo.Id == userNotificationId
                        select new
                        {
                            userNotificationInfo,
                            tenantNotificationInfo
                        };

                    var item = query.FirstOrDefault();
                    if (item == null)
                    {
                        await uow.CompleteAsync();
                        return null;
                    }

                    await uow.CompleteAsync();
                    return new UserNotificationInfoWithNotificationInfo(
                        item.userNotificationInfo,
                        item.tenantNotificationInfo
                    );
                }
            }
        }

        public virtual UserNotificationInfoWithNotificationInfo GetUserNotificationWithNotificationOrNull(
            int? tenantId,
            Guid userNotificationId)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var query = from userNotificationInfo in _userNotificationRepository.GetAll()
                        join tenantNotificationInfo in _tenantNotificationRepository.GetAll() on userNotificationInfo
                            .TenantNotificationId equals tenantNotificationInfo.Id
                        where userNotificationInfo.Id == userNotificationId
                        select new
                        {
                            userNotificationInfo,
                            tenantNotificationInfo
                        };

                    var item = query.FirstOrDefault();
                    if (item == null)
                    {
                        uow.Complete();
                        return null;
                    }

                    uow.Complete();
                    return new UserNotificationInfoWithNotificationInfo(
                        item.userNotificationInfo,
                        item.tenantNotificationInfo
                    );
                }
            }
        }

        public virtual async Task InsertTenantNotificationAsync(TenantNotificationInfo tenantNotificationInfo)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantNotificationInfo.TenantId))
                {
                    await _tenantNotificationRepository.InsertAsync(tenantNotificationInfo);
                }

                await uow.CompleteAsync();
            }
        }

        public virtual void InsertTenantNotification(TenantNotificationInfo tenantNotificationInfo)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantNotificationInfo.TenantId))
                {
                    _tenantNotificationRepository.Insert(tenantNotificationInfo);
                }

                uow.Complete();
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
