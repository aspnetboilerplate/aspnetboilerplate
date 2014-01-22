using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Users;

namespace Abp.Roles
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRoleService(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        public IReadOnlyCollection<Role> GetRolesOfUser(User user)
        {
            return _userRoleRepository
                .GetRolesOfUser(user.Id)
                .ToImmutableList();
        }
    }
}