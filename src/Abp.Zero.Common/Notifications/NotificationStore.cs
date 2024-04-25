using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq;
using Abp.Linq.Expressions;
using Abp.Linq.Extensions;

namespace Abp.Notifications
{
    /// <summary>
    /// Implements <see cref="INotificationStore"/> using repositories.
    /// </summary>
    public class NotificationStore : INotificationStore, ITransientDependency
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        private readonly IRepository<NotificationInfo, Guid> _notificationRepository;
        private readonly IRepository<TenantNotificationInfo, Guid> _tenantNotificationRepository;
        private readonly IRepository<UserNotificationInfo, Guid> _userNotificationRepository;
        private readonly IRepository<NotificationSubscriptionInfo, Guid> _notificationSubscriptionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;

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

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public virtual async Task InsertSubscriptionAsync(NotificationSubscriptionInfo subscription)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(subscription.TenantId))
                {
                    await _notificationSubscriptionRepository.InsertAsync(subscription);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        public virtual void InsertSubscription(NotificationSubscriptionInfo subscription)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(subscription.TenantId))
                {
                    _notificationSubscriptionRepository.Insert(subscription);
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        public virtual async Task DeleteSubscriptionAsync(
            UserIdentifier user,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
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
            });
        }

        public virtual void DeleteSubscription(
            UserIdentifier user,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
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
            });
        }

        public virtual async Task InsertNotificationAsync(NotificationInfo notification)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    await _notificationRepository.InsertAsync(notification);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        public virtual void InsertNotification(NotificationInfo notification)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    _notificationRepository.Insert(notification);
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        public virtual async Task<NotificationInfo> GetNotificationOrNullAsync(Guid notificationId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    return await _notificationRepository.FirstOrDefaultAsync(notificationId);
                }
            });
        }

        public virtual NotificationInfo GetNotificationOrNull(Guid notificationId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    return _notificationRepository.FirstOrDefault(notificationId);
                }
            });
        }

        public virtual async Task InsertUserNotificationAsync(UserNotificationInfo userNotification)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(userNotification.TenantId))
                {
                    await _userNotificationRepository.InsertAsync(userNotification);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        public virtual void InsertUserNotification(UserNotificationInfo userNotification)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(userNotification.TenantId))
                {
                    _userNotificationRepository.Insert(userNotification);
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        public virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(
            string notificationName,
            string entityTypeName,
            string entityId,
            string targetNotifiers)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var predicate = GetNotificationSubscriptionPredicate(
                        notificationName,
                        entityTypeName,
                        entityId,
                        targetNotifiers
                    );

                    return await _notificationSubscriptionRepository.GetAllListAsync(predicate);
                }
            });
        }

        public virtual List<NotificationSubscriptionInfo> GetSubscriptions(
            string notificationName,
            string entityTypeName,
            string entityId,
            string targetNotifiers)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var predicate = GetNotificationSubscriptionPredicate(
                        notificationName,
                        entityTypeName,
                        entityId,
                        targetNotifiers
                    );

                    return _notificationSubscriptionRepository.GetAllList(predicate);
                }
            });
        }

        public virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(
            int?[] tenantIds,
            string notificationName,
            string entityTypeName,
            string entityId,
            string targetNotifiers)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var subscriptions = new List<NotificationSubscriptionInfo>();

                foreach (var tenantId in tenantIds)
                {
                    subscriptions.AddRange(
                        await GetSubscriptionsAsync(
                            tenantId,
                            notificationName,
                            entityTypeName,
                            entityId,
                            targetNotifiers
                        )
                    );
                }

                return subscriptions;
            });
        }

        public virtual List<NotificationSubscriptionInfo> GetSubscriptions(
            int?[] tenantIds,
            string notificationName,
            string entityTypeName,
            string entityId,
            string targetNotifiers)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var subscriptions = new List<NotificationSubscriptionInfo>();

                foreach (var tenantId in tenantIds)
                {
                    subscriptions.AddRange(
                        GetSubscriptions(
                            tenantId,
                            notificationName,
                            entityTypeName,
                            entityId,
                            targetNotifiers
                        )
                    );
                }

                return subscriptions;
            });
        }

        public virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(UserIdentifier user)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    return await _notificationSubscriptionRepository.GetAllListAsync(s => s.UserId == user.UserId);
                }
            });
        }

        public virtual List<NotificationSubscriptionInfo> GetSubscriptions(UserIdentifier user)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    return _notificationSubscriptionRepository.GetAllList(s => s.UserId == user.UserId);
                }
            });
        }

        protected virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(
            int? tenantId,
            string notificationName,
            string entityTypeName,
            string entityId,
            string targetNotifiers)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var predicate = GetNotificationSubscriptionPredicate(
                        notificationName,
                        entityTypeName,
                        entityId,
                        targetNotifiers
                    );

                    return await _notificationSubscriptionRepository.GetAllListAsync(predicate);
                }
            });
        }

        protected virtual ExpressionStarter<NotificationSubscriptionInfo> GetNotificationSubscriptionPredicate(
            string notificationName, string entityTypeName,
            string entityId, string targetNotifiers)
        {
            var predicate = PredicateBuilder.New<NotificationSubscriptionInfo>();
            predicate = predicate.And(e => e.NotificationName == notificationName);
            predicate = predicate.And(e => e.EntityTypeName == entityTypeName);
            predicate = predicate.And(e => e.EntityId == entityId);

            if (!targetNotifiers.IsNullOrEmpty())
            {
                var targetNotifierPredicate = PredicateBuilder.New<NotificationSubscriptionInfo>();
                var targetNotifierList = targetNotifiers.Split(NotificationInfo.NotificationTargetSeparator);
                foreach (var targetNotifier in targetNotifierList)
                {
                    targetNotifierPredicate = targetNotifierPredicate.Or(e => e.TargetNotifiers.Contains(targetNotifier));
                }
                
                predicate = predicate.And(targetNotifierPredicate);
            }

            return predicate;
        }

        protected virtual List<NotificationSubscriptionInfo> GetSubscriptions(
            int? tenantId,
            string notificationName,
            string entityTypeName,
            string entityId,
            string targetNotifiers)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var predicate = GetNotificationSubscriptionPredicate(
                        notificationName,
                        entityTypeName,
                        entityId,
                        targetNotifiers
                    );

                    return _notificationSubscriptionRepository.GetAllList(predicate);
                }
            });
        }

        public virtual async Task<bool> IsSubscribedAsync(
            UserIdentifier user,
            string notificationName,
            string entityTypeName,
            string entityId,
            string targetNotifiers)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var predicate = GetNotificationSubscriptionPredicate(
                        notificationName,
                        entityTypeName,
                        entityId,
                        targetNotifiers
                    );

                    predicate = predicate.And(e => e.UserId == user.UserId);

                    return await _notificationSubscriptionRepository.CountAsync(predicate) > 0;
                }
            });
        }

        public virtual bool IsSubscribed(
            UserIdentifier user,
            string notificationName,
            string entityTypeName,
            string entityId,
            string targetNotifiers)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var predicate = GetNotificationSubscriptionPredicate(
                        notificationName,
                        entityTypeName,
                        entityId,
                        targetNotifiers
                    );

                    predicate = predicate.And(e => e.UserId == user.UserId);

                    return _notificationSubscriptionRepository.Count(predicate) > 0;
                }
            });
        }

        public virtual async Task UpdateUserNotificationStateAsync(
            int? tenantId,
            Guid userNotificationId,
            UserNotificationState state)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
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
            });
        }

        public virtual void UpdateUserNotificationState(
            int? tenantId,
            Guid userNotificationId,
            UserNotificationState state)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
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
            });
        }

        public virtual async Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
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
            });
        }

        public virtual void UpdateAllUserNotificationStates(UserIdentifier user, UserNotificationState state)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
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
            });
        }

        public virtual async Task DeleteUserNotificationAsync(int? tenantId, Guid userNotificationId)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await _userNotificationRepository.DeleteAsync(userNotificationId);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        public virtual void DeleteUserNotification(int? tenantId, Guid userNotificationId)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    _userNotificationRepository.Delete(userNotificationId);
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        public virtual async Task DeleteAllUserNotificationsAsync(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);

                    await _userNotificationRepository.DeleteAsync(predicate);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        public virtual void DeleteAllUserNotifications(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);

                    _userNotificationRepository.Delete(predicate);
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
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
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var query = from userNotificationInfo in await _userNotificationRepository.GetAllAsync()
                        join tenantNotificationInfo in await _tenantNotificationRepository.GetAllAsync() on userNotificationInfo
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

                    var list = await _asyncQueryableExecuter.ToListAsync(query);

                    return list.Select(
                        a => new UserNotificationInfoWithNotificationInfo(
                            a.userNotificationInfo,
                            a.tenantNotificationInfo
                        )
                    ).ToList();
                }
            });
        }

        public virtual List<UserNotificationInfoWithNotificationInfo> GetUserNotificationsWithNotifications(
            UserIdentifier user,
            UserNotificationState? state = null,
            int skipCount = 0,
            int maxResultCount = int.MaxValue,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
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

                    return list.Select(
                        a => new UserNotificationInfoWithNotificationInfo(
                            a.userNotificationInfo,
                            a.tenantNotificationInfo
                        )
                    ).ToList();
                }
            });
        }

        public virtual async Task<int> GetUserNotificationCountAsync(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);
                    return await _userNotificationRepository.CountAsync(predicate);
                }
            });
        }

        public virtual int GetUserNotificationCount(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);
                    return _userNotificationRepository.Count(predicate);
                }
            });
        }

        public virtual async Task<UserNotificationInfoWithNotificationInfo>
            GetUserNotificationWithNotificationOrNullAsync(
                int? tenantId,
                Guid userNotificationId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var query = from userNotificationInfo in await _userNotificationRepository.GetAllAsync()
                        join tenantNotificationInfo in await _tenantNotificationRepository.GetAllAsync() on userNotificationInfo
                            .TenantNotificationId equals tenantNotificationInfo.Id
                        where userNotificationInfo.Id == userNotificationId
                        select new
                        {
                            userNotificationInfo,
                            tenantNotificationInfo
                        };

                    var item = await _asyncQueryableExecuter.FirstOrDefaultAsync(query);
                    if (item == null)
                    {
                        return null;
                    }

                    return new UserNotificationInfoWithNotificationInfo(
                        item.userNotificationInfo,
                        item.tenantNotificationInfo
                    );
                }
            });
        }

        public virtual UserNotificationInfoWithNotificationInfo GetUserNotificationWithNotificationOrNull(
            int? tenantId,
            Guid userNotificationId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
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
                        return null;
                    }

                    return new UserNotificationInfoWithNotificationInfo(
                        item.userNotificationInfo,
                        item.tenantNotificationInfo
                    );
                }
            });
        }

        public virtual async Task InsertTenantNotificationAsync(TenantNotificationInfo tenantNotificationInfo)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantNotificationInfo.TenantId))
                {
                    await _tenantNotificationRepository.InsertAsync(tenantNotificationInfo);
                }
            });
        }

        public virtual void InsertTenantNotification(TenantNotificationInfo tenantNotificationInfo)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantNotificationInfo.TenantId))
                {
                    _tenantNotificationRepository.Insert(tenantNotificationInfo);
                }
            });
        }

        public virtual async Task DeleteNotificationAsync(NotificationInfo notification)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _notificationRepository.DeleteAsync(notification));
        }

        public virtual void DeleteNotification(NotificationInfo notification)
        {
            _unitOfWorkManager.WithUnitOfWork(() => { _notificationRepository.Delete(notification); });
        }

        public async Task<List<GetNotificationsCreatedByUserOutput>> GetNotificationsPublishedByUserAsync(
            UserIdentifier user, string notificationName, DateTime? startDate, DateTime? endDate)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    var queryForNotPublishedNotifications = (await _notificationRepository.GetAllAsync())
                        .Where(n => n.CreatorUserId == user.UserId && n.NotificationName == notificationName);

                    if (startDate.HasValue)
                    {
                        queryForNotPublishedNotifications = queryForNotPublishedNotifications
                            .Where(x => x.CreationTime >= startDate);
                    }

                    if (endDate.HasValue)
                    {
                        queryForNotPublishedNotifications = queryForNotPublishedNotifications
                            .Where(x => x.CreationTime <= endDate);
                    }

                    var result = new List<GetNotificationsCreatedByUserOutput>();

                    var unPublishedNotifications = await AsyncQueryableExecuter.ToListAsync(
                        queryForNotPublishedNotifications
                            .Select(x =>
                                new GetNotificationsCreatedByUserOutput()
                                {
                                    Data = x.Data,
                                    Severity = x.Severity,
                                    NotificationName = x.NotificationName,
                                    DataTypeName = x.DataTypeName,
                                    IsPublished = false,
                                    CreationTime = x.CreationTime
                                })
                    );

                    result.AddRange(unPublishedNotifications);

                    var queryForPublishedNotifications = _tenantNotificationRepository.GetAll()
                        .Where(n => n.CreatorUserId == user.UserId && n.NotificationName == notificationName);

                    if (startDate.HasValue)
                    {
                        queryForPublishedNotifications = queryForPublishedNotifications
                            .Where(x => x.CreationTime >= startDate);
                    }

                    if (endDate.HasValue)
                    {
                        queryForPublishedNotifications = queryForPublishedNotifications
                            .Where(x => x.CreationTime <= endDate);
                    }

                    queryForPublishedNotifications = queryForPublishedNotifications
                        .OrderByDescending(n => n.CreationTime);

                    var publishedNotifications = await AsyncQueryableExecuter.ToListAsync(queryForPublishedNotifications
                        .Select(x =>
                            new GetNotificationsCreatedByUserOutput()
                            {
                                Data = x.Data,
                                Severity = x.Severity,
                                NotificationName = x.NotificationName,
                                DataTypeName = x.DataTypeName,
                                IsPublished = true,
                                CreationTime = x.CreationTime
                            })
                    );

                    result.AddRange(publishedNotifications);
                    return result;
                }
            });
        }
    }
}