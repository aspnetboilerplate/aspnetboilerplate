using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Utils.Extensions.Collections;

namespace Abp.Security.Roles
{
    public class UserRoleManager : IUserRoleManager
    {
        private readonly IUserRoleRepository _userRoleRepository;

        private readonly ConcurrentDictionary<int, UserRoleInfo> _roles;

        public UserRoleManager(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
            _roles = new ConcurrentDictionary<int, UserRoleInfo>();
        }
        
        public IReadOnlyList<string> GetRolesOfUser(int userId)
        {
            UserRoleInfo roleInfo = _roles.GetOrDefault(userId);
            if (roleInfo == null)
            {
                lock (_roles)
                {
                    roleInfo = _roles.GetOrDefault(userId);
                    if (roleInfo == null)
                    {
                        roleInfo = new UserRoleInfo
                                   {
                                       UserId = userId,
                                       Roles = _userRoleRepository.Query(q =>q.Where(ur => ur.User.Id == userId).Select(ur => ur.Role.Name).ToList())
                                   };
                    }
                }
            }

            return roleInfo.Roles.ToImmutableList();
        }

        private class UserRoleInfo
        {
            public int UserId { get; set; }

            public List<string> Roles { get; set; }
        }
    }
}