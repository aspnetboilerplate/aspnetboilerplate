using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Users;
using Microsoft.AspNet.Identity;

namespace Abp.Web.Identity
{
    public class UserStoreAdapter : IUserStore<UserAdapter, int>
    {
        private readonly IUserRepository _userRepository;

        private bool _isDisposed;

        public UserStoreAdapter(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            IocHelper.Release(this);
        }

        public Task CreateAsync(UserAdapter user)
        {
            return new Task(() => { });
        }

        public Task UpdateAsync(UserAdapter user)
        {
            return new Task(() => { });
        }

        public Task DeleteAsync(UserAdapter user)
        {
            return new Task(() => _userRepository.Delete(user.Id));
        }

        public Task<UserAdapter> FindByIdAsync(int userId)
        {
            return new Task<UserAdapter>(() => new UserAdapter(_userRepository.FirstOrDefault(userId)));
        }

        public Task<UserAdapter> FindByNameAsync(string userName)
        {
            return new Task<UserAdapter>(() => new UserAdapter(_userRepository.FirstOrDefault(u => u.EmailAddress == userName)));
        }
    }
}
