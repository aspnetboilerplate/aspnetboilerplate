using System.Threading.Tasks;
using Abp.Application.Services;
using MyAbpZeroProject.Users.Dto;
using System.Collections.Generic;

namespace MyAbpZeroProject.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task ProhibitPermission(ProhibitPermissionInput input);

        Task RemoveFromRole(long userId, string roleName);

        Task<List<User>> GetAllUsers();
    }
}