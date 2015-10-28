using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using MyAbpZeroProject.Authorization.Roles;
using MyAbpZeroProject.MultiTenancy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyAbpZeroProject.Users
{
    public class UserStore : AbpUserStore<Tenant, Role, User>
    {
        private readonly IRepository<User, long> _userRepository;
        public UserStore(
            
        IRepository<User, long> userRepository,
            IRepository<UserLogin, long> userLoginRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository,
            IRepository<UserPermissionSetting, long> userPermissionSettingRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ICacheManager cacheManager)
            : base(
              userRepository,
              userLoginRepository,
              userRoleRepository,
              roleRepository,
              userPermissionSettingRepository,
              unitOfWorkManager,
              cacheManager)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllListAsync();
        }
    }
}