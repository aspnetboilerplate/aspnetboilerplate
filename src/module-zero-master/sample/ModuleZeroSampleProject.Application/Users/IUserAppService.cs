using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ModuleZeroSampleProject.Users.Dto;

namespace ModuleZeroSampleProject.Users
{
    public interface IUserAppService : IApplicationService
    {
        ListResultOutput<UserDto> GetUsers();
    }
}
