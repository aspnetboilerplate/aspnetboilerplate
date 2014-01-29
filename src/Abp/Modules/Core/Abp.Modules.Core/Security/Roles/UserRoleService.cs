using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Security.Users;

namespace Abp.Security.Roles
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRoleService(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        public IReadOnlyCollection<AbpRole> GetRolesOfUser(AbpUser user)
        {
            return _userRoleRepository
                .GetRolesOfUser(user.Id)
                .ToImmutableList();
        }
    }
}