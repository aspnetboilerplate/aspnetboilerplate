using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.UI;
using Abp.Zero.SampleApp.Users.Dto;
using Microsoft.AspNet.Identity;

namespace Abp.Zero.SampleApp.Users
{
    public class UserAppService : IUserAppService
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserManager _userManager;

        public UserAppService(
            IRepository<User, long> userRepository,
            IUnitOfWorkManager unitOfWorkManager,
            UserManager userManager)
        {
            _userRepository = userRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _userManager = userManager;
        }

        public void CreateUser(CreateUserInput input)
        {
            _userManager.Create(new User
            {
                TenantId = null,
                UserName = input.UserName,
                Name = input.Name,
                Surname = input.Surname,
                EmailAddress = input.EmailAddress,
                IsEmailConfirmed = true,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
            });
        }

        public void UpdateUser(UpdateUserInput input)
        {
            var user = _userRepository.Get(input.Id);

            user.TenantId = null;
            user.UserName = input.UserName;
            user.Name = input.Name;
            user.Surname = input.Surname;
            user.EmailAddress = input.EmailAddress;
            user.IsEmailConfirmed = true;
            user.Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw=="; //123qwe

            _userRepository.Update(user);
        }

        public void DeleteUser(long userId)
        {
            _userRepository.Delete(userId);
        }

        public virtual async Task ResetPassword(ResetPasswordInput input)
        {
            _unitOfWorkManager.Current.SetTenantId(input.TenantId);

            var user = await _userManager.GetUserByIdAsync(input.UserId);
            if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != input.ResetCode)
            {
                throw new UserFriendlyException("InvalidPasswordResetCode", "InvalidPasswordResetCode_Detail");
            }

            user.Password = new PasswordHasher().HashPassword(input.Password);
            user.PasswordResetCode = null;
            user.IsEmailConfirmed = true;

            await _userManager.UpdateAsync(user);
        }

        public void CustomValidateMethod(CustomValidateMethodInput input)
        {
            //Becasue of the validation error in AddValidationErrors method of CustomValidateMethodInput, 
            //this method will not be called.
        }
    }
}