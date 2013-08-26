using System.Collections.Generic;
using Abp.Entities.Core;
using Abp.Services.Core.Dto;

namespace Abp.Services.Core
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
