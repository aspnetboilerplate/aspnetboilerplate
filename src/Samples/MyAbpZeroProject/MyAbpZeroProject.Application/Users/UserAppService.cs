using System.Threading.Tasks;
using Abp.Authorization;
using MyAbpZeroProject.Users.Dto;
using AutoMapper;
using System.Collections.Generic;

namespace MyAbpZeroProject.Users
{
    /* THIS IS JUST A SAMPLE. */
    public class UserAppService : MyAbpZeroProjectAppServiceBase, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly IPermissionManager _permissionManager;
        private readonly UserStore _userStore;

        public UserAppService(UserManager userManager, IPermissionManager permissionManager, UserStore userStore)
        {
            _userManager = userManager;
            _permissionManager = permissionManager;
            _userStore = userStore;
        }

        public async Task ProhibitPermission(ProhibitPermissionInput input)
        {
            var user = await _userManager.GetUserByIdAsync(input.UserId);
            var permission = _permissionManager.GetPermission(input.PermissionName);

            await _userManager.ProhibitPermissionAsync(user, permission);
        }

        //Example for primitive method parameters.
        public async Task RemoveFromRole(long userId, string roleName)
        {
            CheckErrors(await _userManager.RemoveFromRoleAsync(userId, roleName));
        }
        public async Task<List<User>> GetAllUsers()
        {
          return await _userStore.GetAllUsersAsync();
            //return new GetAllUsersOutput
            //{
            //    Users = Mapper.Map<List<GetAllUsersOutput>>(await _userStore.GetAllUsersAsync())
            //};
        }
    }
}