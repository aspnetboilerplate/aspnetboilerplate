using Abp.Application.Services;

namespace Abp.ZeroCore.SampleApp.Application.Users;

public interface IUserAppService : IAsyncCrudAppService<UserDto, long>
{

}