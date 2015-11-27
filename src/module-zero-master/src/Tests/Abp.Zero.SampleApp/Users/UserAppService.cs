using Abp.Domain.Repositories;
using Abp.Zero.SampleApp.Users.Dto;

namespace Abp.Zero.SampleApp.Users
{
    public class UserAppService : IUserAppService
    {
        private readonly IRepository<User, long> _userRepository;

        public UserAppService(IRepository<User, long> userRepository)
        {
            _userRepository = userRepository;
        }

        public void CreateUser(CreateUserInput input)
        {
            _userRepository.Insert(new User
                                   {
                                       TenantId = null,
                                       UserName = input.UserName,
                                       Name = input.Name,
                                       Surname = input.Surname,
                                       EmailAddress = input.EmailAddress,
                                       IsEmailConfirmed = true,
                                       Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
                                   });
        }
    }
}