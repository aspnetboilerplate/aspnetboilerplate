using System.Collections.Generic;
using System.Linq;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Entities;
using Abp.Modules.Core.Services.Dto;

namespace Abp.Modules.Core.Services.Impl
{
    /// <summary>
    /// Implementation of IUserService interface.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository questionRepository)
        {
            _userRepository = questionRepository;
        }

        /// <summary>
        /// NOTE: this is for test purpose!
        /// </summary>
        /// <returns>List of all users</returns>
        public IList<UserDto> GetAllUsers()
        {
            return _userRepository.Query(q => q.ToList()).MapIList<User, UserDto>();
        }
    }
}