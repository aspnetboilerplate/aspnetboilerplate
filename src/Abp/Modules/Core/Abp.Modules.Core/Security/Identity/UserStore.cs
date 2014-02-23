using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Security.Roles;
using Abp.Security.Users;
using Microsoft.AspNet.Identity;

namespace Abp.Security.Identity
{
    public class UserStore<TUser, TUserRepository> :
        IUserPasswordStore<TUser, int>,
        IUserEmailStore<TUser, int>,
        IUserLoginStore<TUser, int>,
        IUserRoleStore<TUser, int>,
        IUserConfirmationStore<TUser, int>,
        ITransientDependency
        where TUser : AbpUser
        where TUserRepository : IUserRepository<TUser>
    {
        #region Private fields

        private readonly TUserRepository _userRepository;
        private readonly IUserLoginRepository<TUser> _userLoginRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IAbpRoleRepository _abpRoleRepository;

        #endregion

        #region Constructor

        public UserStore(
            TUserRepository userRepository,
            IUserLoginRepository<TUser> userLoginRepository,
            IUserRoleRepository userRoleRepository,
            IAbpRoleRepository abpRoleRepository)
        {
            _userRepository = userRepository;
            _userLoginRepository = userLoginRepository;
            _userRoleRepository = userRoleRepository;
            _abpRoleRepository = abpRoleRepository;
        }

        #endregion

        #region IUserStore

        public void Dispose()
        {
            //No need to dispose since using dependency injection manager
        }

        public Task CreateAsync(TUser user)
        {
            return Task.Factory.StartNew(() => _userRepository.Insert(user));
        }

        public Task UpdateAsync(TUser user)
        {
            return Task.Factory.StartNew(() => _userRepository.Update(user));
        }

        public Task DeleteAsync(TUser user)
        {
            return Task.Factory.StartNew(() => _userRepository.Delete(user.Id));
        }

        public Task<TUser> FindByIdAsync(int userId)
        {
            return Task.Factory.StartNew(() => _userRepository.FirstOrDefault(userId));
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            return Task.Factory.StartNew(() => _userRepository.FirstOrDefault(user => user.UserName == userName || user.EmailAddress == userName));
        }

        #endregion

        #region IUserPasswordStore

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            return Task.Factory.StartNew(() => _userRepository.UpdatePassword(user.Id, passwordHash));
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.Factory.StartNew(() => _userRepository.Get(user.Id).Password); //TODO: Optimize
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.Factory.StartNew(() => !string.IsNullOrEmpty(_userRepository.Get(user.Id).Password)); //TODO: Optimize
        }

        #endregion

        #region IUserEmailStore

        public Task SetEmailAsync(TUser user, string email)
        {
            return Task.Factory.StartNew(() => _userRepository.UpdateEmail(user.Id, email));
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.Factory.StartNew(() => _userRepository.Get(user.Id).EmailAddress);
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            return Task.Factory.StartNew(() => _userRepository.FirstOrDefault(user => user.EmailAddress == email));
        }

        #endregion

        #region IUserLoginStore

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            //TODO: Check if already exists?
            return Task.Factory.StartNew(
                () =>
                    _userLoginRepository.Insert(
                        new UserLogin
                        {
                            LoginProvider = login.LoginProvider,
                            ProviderKey = login.ProviderKey,
                            UserId = user.Id
                        })
                );
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            throw new NotImplementedException(); //TODO: Implement!
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            return Task.Factory.StartNew<IList<UserLoginInfo>>(
                () =>
                    _userLoginRepository
                        .GetAllList(ul => ul.UserId == user.Id)
                        .Select(ul => new UserLoginInfo(ul.LoginProvider, ul.ProviderKey))
                        .ToList()
                );
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            return Task.Factory.StartNew(
                () => FindUser(login.LoginProvider, login.ProviderKey)
                );
        }

        [UnitOfWork]
        protected virtual TUser FindUser(string loginProvider, string providerKey)
        {
            var query =
                from user in _userRepository.GetAll()
                join userLogin in _userLoginRepository.GetAll() on user.Id equals userLogin.UserId
                where userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey
                select user;
            return query.FirstOrDefault();
        }

        #endregion

        #region IUserRoleStore

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            //TODO: Check if already exists?
            return Task.Factory.StartNew(
                () =>
                    _userRoleRepository.Insert(
                        new UserRole
                        {
                            User = user,
                            Role = _abpRoleRepository.Single(role => role.Name == roleName) //TODO: Can find another way?
                        })
                );
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    var userRole = _userRoleRepository.FirstOrDefault(
                        ur => ur.User.Id == user.Id && ur.Role.Name == roleName
                        );

                    if (userRole == null)
                    {
                        return;
                    }

                    _userRoleRepository.Delete(userRole.Id);
                });
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            return Task.Factory.StartNew<IList<string>>(
                () =>
                    _userRoleRepository
                        .Query(q => q
                            .Where(ur => ur.User.Id == user.Id)
                            .Select(ur => ur.Role.Name)
                            .ToList()
                        )
                );
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            return Task.Factory.StartNew(
                () => _userRoleRepository.FirstOrDefault(
                    ur => ur.User.Id == user.Id && ur.Role.Name == roleName
                    ) != null
                );
        }

        #endregion

        #region IUserConfirmationStore

        public Task<bool> IsConfirmedAsync(TUser user)
        {
            return Task.Factory.StartNew(() => _userRepository.Get(user.Id).IsEmailConfirmed); //TODO: Optimize
        }

        public Task SetConfirmedAsync(TUser user, bool confirmed)
        {
            return Task.Factory.StartNew(() => _userRepository.UpdateIsEmailConfirmed(user.Id, confirmed));
        }

        #endregion
    }
}
