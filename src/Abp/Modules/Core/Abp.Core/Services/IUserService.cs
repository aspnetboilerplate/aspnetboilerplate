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
        /// <summary>
        /// NOTE: this is for test purpose!
        /// </summary>
        /// <returns></returns>
        IList<UserDto> GetAllUsers();
    }
}
