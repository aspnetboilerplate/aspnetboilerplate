using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Abp.Domain.Uow;
using Abp.Exceptions;
using Abp.Modules.Core.Application.Services.Dto;
using Abp.Modules.Core.Application.Services.Dto.Users;
using Abp.Modules.Core.Domain.Entities;
using Abp.Modules.Core.Domain.Repositories;

namespace Abp.Modules.Core.Application.Services.Impl
{
    /// <summary>
    /// Implementation of IUserService interface.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository questionRepository, ITenantRepository tenantRepository, IEmailService emailService)
        {
            _userRepository = questionRepository;
            _tenantRepository = tenantRepository;
            _emailService = emailService;
        }

        public IList<UserDto> GetAllUsers()
        {
            return _userRepository.Query(q => q.ToList()).MapIList<User, UserDto>();
        }

        public UserDto GetActiveUserOrNull(string emailAddress, string password) //TODO: Make this GetUserOrNullInput and GetUserOrNullOutput
        {
            var userEntity = _userRepository.Query(q => q.FirstOrDefault(user => user.EmailAddress == emailAddress && user.Password == password && user.IsEmailConfirmed));
            return userEntity.MapTo<UserDto>();
        }

        public GetUserOutput GetUser(GetUserInput input)
        {
            var user = _userRepository.Get(input.UserId);
            return new GetUserOutput(user.MapTo<UserDto>());
        }

        [UnitOfWork]
        public void RegisterUser(RegisterUserInput registerUser)
        {
            var existingUser = _userRepository.Query(q => q.FirstOrDefault(u => u.EmailAddress == registerUser.EmailAddress));
            if (existingUser != null)
            {
                if (!existingUser.IsEmailConfirmed)
                {
                    SendConfirmationEmail(existingUser);
                    throw new AbpUserFriendlyException("You registere with this email address before (" + registerUser.EmailAddress + ")! We re-sent an activation code to your email!");
                }

                throw new AbpUserFriendlyException("There is already a user with this email address (" + registerUser.EmailAddress + ")! Select another email address!");
            }

            var userEntity = registerUser.MapTo<User>();
            userEntity.Tenant = _tenantRepository.Load(1); //TODO: Get from subdomain or ?
            userEntity.GenerateEmailConfirmationCode();
            _userRepository.Insert(userEntity);
            SendConfirmationEmail(userEntity);
        }

        [UnitOfWork]
        public void ConfirmEmail(ConfirmEmailInput input)
        {
            var user = _userRepository.Get(input.UserId);
            user.ConfirmEmail(input.ConfirmationCode);
        }

        public GetCurrentUserInfoOutput GetCurrentUserInfo(GetCurrentUserInfoInput input)
        {
            //TODO: Use GetUser?
            return new GetCurrentUserInfoOutput { User = _userRepository.Get(User.CurrentUserId).MapTo<UserDto>() };
        }

        [UnitOfWork]
        public void ChangePassword(ChangePasswordInput input)
        {
            var currentUser = _userRepository.Get(User.CurrentUserId);
            if (currentUser.Password != input.CurrentPassword)
            {
                throw new AbpUserFriendlyException("Current password is invalid!");
            }

            currentUser.Password = input.NewPassword;
        }

        [UnitOfWork]
        public ChangeProfileImageOutput ChangeProfileImage(ChangeProfileImageInput input)
        {
            var currentUser = _userRepository.Get(User.CurrentUserId); //TODO: test Load method
            var oldFileName = currentUser.ProfileImage;

            currentUser.ProfileImage = input.FileName;

            return new ChangeProfileImageOutput() { OldFileName = oldFileName };
        }

        [UnitOfWork]
        public void SendPasswordResetLink(SendPasswordResetLinkInput input)
        {
            var currentUser = _userRepository.Get(User.CurrentUserId);
            currentUser.GeneratePasswordResetCode();
            SendPasswordResetLinkEmail(currentUser);
        }

        [UnitOfWork]
        public void ResetPassword(ResetPasswordInput input)
        {
            var user = _userRepository.Get(input.UserId);
            if (string.IsNullOrWhiteSpace(user.PasswordResetCode))
            {
                throw new AbpUserFriendlyException("You can not reset your password. You must first send a reset password link to your email.");
            }

            if (input.PasswordResetCode != user.PasswordResetCode)
            {
                throw new AbpUserFriendlyException("Password reset code is invalid!");
            }

            user.Password = input.Password;
            user.PasswordResetCode = null;
        }

        private void SendConfirmationEmail(User user)
        {
            var mail = new MailMessage();
            mail.To.Add(user.EmailAddress);
            mail.IsBodyHtml = true;
            mail.Subject = "Email confirmation for Taskever";
            mail.SubjectEncoding = Encoding.UTF8;

            var mailBuilder = new StringBuilder();
            mailBuilder.Append(
@"<!DOCTYPE html>
<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta charset=""utf-8"" />
    <title>{TEXT_HTML_TITLE}</title>
    <style>
        body {
            font-family: Verdana, Geneva, 'DejaVu Sans', sans-serif;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <h3>{TEXT_WELCOME}</h3>
    <p>{TEXT_DESCRIPTION}</p>
    <p>&nbsp;</p>
    <p><a href=""http://www.taskever.com/Account/ConfirmEmail?UserId={USER_ID}&ConfirmationCode={CONFIRMATION_CODE}"">http://www.taskever.com/Account/ConfirmEmail?UserId={USER_ID}&amp;ConfirmationCode={CONFIRMATION_CODE}</a></p>
    <p>&nbsp;</p>
    <p><a href=""http://www.taskever.com"">http://www.taskever.com</a></p>
</body>
</html>");

            mailBuilder.Replace("{TEXT_HTML_TITLE}", "Email confirmation for Taskever");
            mailBuilder.Replace("{TEXT_WELCOME}", "Welcome to Taskever.com!");
            mailBuilder.Replace("{TEXT_DESCRIPTION}", "Click the link below to confirm your email address and login to the Taskever.com");
            mailBuilder.Replace("{USER_ID}", user.Id.ToString());
            mailBuilder.Replace("{CONFIRMATION_CODE}", user.EmailConfirmationCode);

            mail.Body = mailBuilder.ToString();
            mail.BodyEncoding = Encoding.UTF8;

            _emailService.SendEmail(mail);
        }

        private void SendPasswordResetLinkEmail(User user)
        {
            var mail = new MailMessage();
            mail.To.Add(user.EmailAddress);
            mail.IsBodyHtml = true;
            mail.Subject = "Password reset for Taskever";
            mail.SubjectEncoding = Encoding.UTF8;

            var mailBuilder = new StringBuilder();
            mailBuilder.Append(
@"<!DOCTYPE html>
<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta charset=""utf-8"" />
    <title>{TEXT_HTML_TITLE}</title>
    <style>
        body {
            font-family: Verdana, Geneva, 'DejaVu Sans', sans-serif;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <h3>{TEXT_WELCOME}</h3>
    <p>{TEXT_DESCRIPTION}</p>
    <p>&nbsp;</p>
    <p><a href=""http://www.taskever.com/Account/ResetPassword?UserId={USER_ID}&ResetCode={RESET_CODE}"">http://www.taskever.com/Account/ResetPassword?UserId={USER_ID}&amp;ResetCode={RESET_CODE}</a></p>
    <p>&nbsp;</p>
    <p><a href=""http://www.taskever.com"">http://www.taskever.com</a></p>
</body>
</html>");

            mailBuilder.Replace("{TEXT_HTML_TITLE}", "Password reset for Taskever");
            mailBuilder.Replace("{TEXT_WELCOME}", "Reset your password on Taskever!");
            mailBuilder.Replace("{TEXT_DESCRIPTION}", "Click the link below to reset your password on the Taskever.com");
            mailBuilder.Replace("{USER_ID}", user.Id.ToString());
            mailBuilder.Replace("{RESET_CODE}", user.PasswordResetCode);

            mail.Body = mailBuilder.ToString();
            mail.BodyEncoding = Encoding.UTF8;

            _emailService.SendEmail(mail);
        }
    }
}