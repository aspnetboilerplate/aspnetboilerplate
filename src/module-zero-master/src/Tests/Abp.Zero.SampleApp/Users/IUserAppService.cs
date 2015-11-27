using Abp.Application.Services;
using Abp.Zero.SampleApp.Users.Dto;

namespace Abp.Zero.SampleApp.Users
{
    public interface IUserAppService : IApplicationService
    {
        void CreateUser(CreateUserInput input);
    }
}
