using Abp.Application.Services;

namespace AbpAspNetCoreDemo.Core.Application.Account;

public interface IAccountAppService : IApplicationService
{
    RegisterOutput Register(RegisterInput input);
}