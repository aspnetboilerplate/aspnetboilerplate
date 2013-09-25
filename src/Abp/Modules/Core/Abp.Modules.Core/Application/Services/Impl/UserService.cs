using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Modules.Core.Application.Services.Dto;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;

namespace Abp.Modules.Core.Application.Services.Impl
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

        public IList<UserDto> GetAllUsers()
        {
            return _userRepository.Query(q => q.ToList()).MapIList<User, UserDto>();
        }

        public UserDto GetUserOrNull(string emailAddress, string password)
        {
            var userEntity = _userRepository.Query(q => q.FirstOrDefault(user => user.EmailAddress == emailAddress && user.Password == password));
            return userEntity.MapTo<UserDto>();
        }

        public UserDto GetUser(int userId)
        {
            var userEntity = _userRepository.Query(q => q.FirstOrDefault(user => user.Id == userId));
            if (userEntity == null)
            {
                throw new ApplicationException("Can not find user with id = " + userId);
            }

            return userId.MapTo<UserDto>();
        }
    }
}