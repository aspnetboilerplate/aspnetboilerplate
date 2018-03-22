using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Zero.SampleApp.Users.Dto;

namespace Abp.Zero.SampleApp.Users
{
    public interface IUserAppService : IApplicationService
    {
        void CreateUser(CreateUserInput input);

        void UpdateUser(UpdateUserInput input);

        void DeleteUser(long userId);

        Task ResetPassword(ResetPasswordInput input);

        void CustomValidateMethod(CustomValidateMethodInput input);
    }
}
