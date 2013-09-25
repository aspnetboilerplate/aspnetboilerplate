using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Modules.Core.Services.Dto;

namespace Abp.Modules.Core.Services
{
    /// <summary>
    /// Used to perform User related operations.
    /// </summary>
    public interface IUserService : IApplicationService
    {
        IList<UserDto> GetAllUsers();

        UserDto GetUserOrNull(string emailAddress, string password);

        UserDto GetUser(int userId);
    }
}
