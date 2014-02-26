using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Users.Dto;
using Taskever.Users.Dto;

namespace Taskever.Users
{
    public interface ITaskeverUserAppService : IApplicationService
    {
        ChangeProfileImageOutput ChangeProfileImage(ChangeProfileImageInput input);

        GetUserProfileOutput GetUserProfile(GetUserProfileInput input);

        IList<UserDto> GetAllUsers();

        UserDto GetActiveUserOrNull(string emailAddress, string password);

        GetUserOutput GetUser(GetUserInput input);

        void RegisterUser(RegisterUserInput registerUser);

        void ConfirmEmail(ConfirmEmailInput input);

        GetCurrentUserInfoOutput GetCurrentUserInfo(GetCurrentUserInfoInput input);

        void ChangePassword(ChangePasswordInput input);

        void SendPasswordResetLink(SendPasswordResetLinkInput input);

        void ResetPassword(ResetPasswordInput input);
    }
}
