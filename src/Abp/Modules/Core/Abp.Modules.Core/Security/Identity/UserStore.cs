using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Security.Users;
using Microsoft.AspNet.Identity;

namespace Abp.Security.Identity
{
    public class UserStore<TUser, TUserRepository> :
        IUserPasswordStore<TUser, int>,
        IUserEmailStore<TUser, int>,
        IUserLoginStore<TUser, int>,
        ITransientDependency
        where TUser : User
        where TUserRepository : IUserRepository<TUser>
    {
        private readonly TUserRepository _userRepository;
        private readonly IUserLoginRepository<TUser> _userLoginRepository;

        public UserStore(TUserRepository userRepository, IUserLoginRepository<TUser> userLoginRepository)
        {
            _userRepository = userRepository;
            _userLoginRepository = userLoginRepository;
        }

        public void Dispose()
        {
            //TODO: Dispose if UserManager is not injected!
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

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            return Task.Factory.StartNew(() => _userRepository.UpdatePassword(user.Id, passwordHash));
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.Factory.StartNew(() => _userRepository.Get(user.Id).Password);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.Factory.StartNew(() => !string.IsNullOrEmpty(_userRepository.Get(user.Id).Password));
        }

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

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            return Task.Factory.StartNew(() => _userLoginRepository.Insert(new UserLogin<TUser> { LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey, User = user }));
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            throw new NotImplementedException(); //TODO: Implement!
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            return Task.Factory.StartNew<IList<UserLoginInfo>>(() => _userLoginRepository.GetAllList(ul => ul.User.Id == user.Id).Select(ul => new UserLoginInfo(ul.LoginProvider, ul.ProviderKey)).ToList());
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            return Task.Factory.StartNew(() =>_userLoginRepository.Query(q => q.Where(ul => ul.ProviderKey == login.ProviderKey && ul.LoginProvider == login.LoginProvider).Select(ul => ul.User).FirstOrDefault()));
        }
    }
}
