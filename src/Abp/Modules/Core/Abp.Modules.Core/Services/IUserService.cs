using System.Collections.Generic;
using Abp.Modules.Core.Services.Dto;
using Abp.Services;

namespace Abp.Modules.Core.Services
{
    /// <summary>
    /// Used to perform User related operations.
    /// </summary>
    public interface IUserService : IService
    {
        IList<UserDto> GetAllUsers();

        UserDto GetUserOrNull(string emailAddress, string password);
    }
}
