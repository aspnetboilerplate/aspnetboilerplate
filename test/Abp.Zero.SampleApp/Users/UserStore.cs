using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.SampleApp.Roles;

namespace Abp.Zero.SampleApp.Users
{
    public class UserStore : AbpUserStore<Role, User>
    {
        public UserStore(
            IRepository<User, long> userRepository,
            IRepository<UserLogin, long> userLoginRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository,
            IRepository<UserPermissionSetting, long> userPermissionSettingRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserClaim, long> userClaimRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<RoleOrganizationUnit, long> roleOrganizationUnitRepository)
            : base(
                userRepository,
                userLoginRepository,
                userRoleRepository,
                roleRepository,
                userPermissionSettingRepository,
                unitOfWorkManager,
                userClaimRepository,
                userOrganizationUnitRepository,
                roleOrganizationUnitRepository)
        {

        }
    }
}