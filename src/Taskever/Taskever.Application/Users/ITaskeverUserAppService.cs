using Abp.Application.Services;
using Taskever.Users.Dto;

namespace Taskever.Users
{
    public interface ITaskeverUserAppService : IApplicationService
    {
        GetUserProfileOutput GetUserProfile(GetUserProfileInput input);
    }
}
