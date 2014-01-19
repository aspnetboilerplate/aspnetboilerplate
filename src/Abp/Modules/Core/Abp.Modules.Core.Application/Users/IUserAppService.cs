using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Users.Dto;

namespace Abp.Users
{
    /// <summary>
    /// Used to perform User related operations.
    /// </summary>
    public interface IUserAppService : IApplicationService
    {
        IList<UserDto> GetAllUsers();

        UserDto GetActiveUserOrNull(string emailAddress, string password);

        GetUserOutput GetUser(GetUserInput input);

        void RegisterUser(RegisterUserInput registerUser);

        void ConfirmEmail(ConfirmEmailInput input);

        GetCurrentUserInfoOutput GetCurrentUserInfo(GetCurrentUserInfoInput input);

        void ChangePassword(ChangePasswordInput input);

        ChangeProfileImageOutput ChangeProfileImage(ChangeProfileImageInput input);

        void SendPasswordResetLink(SendPasswordResetLinkInput input);

        void ResetPassword(ResetPasswordInput input);
    }
}
