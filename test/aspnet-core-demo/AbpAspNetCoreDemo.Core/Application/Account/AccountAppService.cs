using Abp.Application.Services;

namespace AbpAspNetCoreDemo.Core.Application.Account;

public class AccountAppService : ApplicationService, IAccountAppService
{
    public RegisterOutput Register(RegisterInput input)
    {
        return new RegisterOutput
        {
            FullName = input.FullName
        };
    }
}