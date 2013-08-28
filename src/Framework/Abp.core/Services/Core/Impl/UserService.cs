using System.Collections.Generic;
using System.Linq;
using Abp.Data.Repositories.Core;
using Abp.Entities.Core;
using Abp.Services.Core.Dto;

namespace Abp.Services.Core.Impl
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
            //Thread.CurrentPrincipal
        }

        /// <summary>
        /// NOTE: this is for test purpose!
        /// </summary>
        /// <returns>List of all users</returns>
        //[AbpAuthorize(Features="")]
        public IList<UserDto> GetAllUsers()
        {
            return _userRepository.Query(q => q.ToList()).MapIList<User, UserDto>();
        }
    }
}