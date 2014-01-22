using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Abp.Localization;
using Abp.Users;

namespace Abp.Roles
{
    public interface IUserRoleService : IDomainService
    {
        IReadOnlyCollection<Role> GetRolesOfUser(User user);
    }
}
