using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;

namespace Abp.Authorization.Users
{
    /// <summary>
    /// Synchronizes a user's information to user account.
    /// </summary>
    public class UserAccountSynchronizer :
        IEventHandler<EntityCreatedEventData<AbpUserBase>>,
        IEventHandler<EntityDeletedEventData<AbpUserBase>>,
        IEventHandler<EntityUpdatedEventData<AbpUserBase>>,
        ITransientDependency
    {
        private readonly IRepository<UserAccount, long> _userAccountRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserAccountSynchronizer(
            IRepository<UserAccount, long> userAccountRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _userAccountRepository = userAccountRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        /// Handles creation event of user
        /// </summary>
        [UnitOfWork]
        public virtual void HandleEvent(EntityCreatedEventData<AbpUserBase> eventData)
        {
            using (_unitOfWorkManager.Current.SetTenantId(null))
            {
                _userAccountRepository.Insert(new UserAccount
                {
                    TenantId = eventData.Entity.TenantId,
                    UserName = eventData.Entity.UserName,
                    UserId = eventData.Entity.Id,
                    EmailAddress = eventData.Entity.EmailAddress
                });
            }
        }

        /// <summary>
        /// Handles deletion event of user
        /// </summary>
        /// <param name="eventData"></param>
        [UnitOfWork]
        public virtual void HandleEvent(EntityDeletedEventData<AbpUserBase> eventData)
        {
            using (_unitOfWorkManager.Current.SetTenantId(null))
            {
                var userAccount =
                    _userAccountRepository.FirstOrDefault(
                        ua => ua.TenantId == eventData.Entity.TenantId && ua.UserId == eventData.Entity.Id);
                if (userAccount != null)
                {
                    _userAccountRepository.Delete(userAccount);
                }
            }
        }

        /// <summary>
        /// Handles update event of user
        /// </summary>
        /// <param name="eventData"></param>
        [UnitOfWork]
        public virtual void HandleEvent(EntityUpdatedEventData<AbpUserBase> eventData)
        {
            using (_unitOfWorkManager.Current.SetTenantId(null))
            {
                var userAccount = _userAccountRepository.FirstOrDefault(ua => ua.TenantId == eventData.Entity.TenantId && ua.UserId == eventData.Entity.Id);
                if (userAccount != null)
                {
                    userAccount.UserName = eventData.Entity.UserName;
                    userAccount.EmailAddress = eventData.Entity.EmailAddress;
                    _userAccountRepository.Update(userAccount);
                }
            }
        }
    }
}