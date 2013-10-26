using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Modules.Core.Application.Services.Dto;
using Abp.Modules.Core.Application.Services.Dto.Users;
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
        private readonly ITenantRepository _tenantRepository;

        public UserService(IUserRepository questionRepository, ITenantRepository tenantRepository)
        {
            _userRepository = questionRepository;
            _tenantRepository = tenantRepository;
        }

        public IList<UserDto> GetAllUsers()
        {
            return _userRepository.Query(q => q.ToList()).MapIList<User, UserDto>();
        }

        public UserDto GetUserOrNull(string emailAddress, string password) //TODO: Make this GetUserOrNullInput and GetUserOrNullOutput
        {
            var userEntity = _userRepository.Query(q => q.FirstOrDefault(user => user.EmailAddress == emailAddress && user.Password == password));
            return userEntity.MapTo<UserDto>();
        }

        public GetUserOutput GetUser(GetUserInput input)
        {
            var user = _userRepository.Get(input.UserId);
            return new GetUserOutput(user.MapTo<UserDto>());
        }

        public void RegisterUser(RegisterUserInput registerUser)
        {
            var userEntity = registerUser.MapTo<User>();
            userEntity.Tenant = _tenantRepository.Load(1); //TODO: Get from subdomain or ?
            _userRepository.Insert(userEntity);
        }

        public GetCurrentUserInfoOutput GetCurrentUserInfo(GetCurrentUserInfoInput input)
        {
            return new GetCurrentUserInfoOutput { User = _userRepository.Get(User.CurrentUserId).MapTo<UserDto>() };
        }
    }
}