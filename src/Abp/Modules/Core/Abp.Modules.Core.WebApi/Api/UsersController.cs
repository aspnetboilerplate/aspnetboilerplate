using System.Collections.Generic;
using System.Web.Http;
using Abp.Modules.Core.Services;
using Abp.Modules.Core.Services.Dto;
using Abp.WebApi.Controllers;

namespace Abp.Modules.Core.Api
{
    public class UsersController : AbpApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public virtual IEnumerable<UserDto> Get()
        {
            //validation
            //throwing appropriate messages
            //logging
            //exception handling
            Logger.Info(L("GetAllQuestions_Method_Is_Called"));
            return _userService.GetAllUsers();
        }

        public int TestMethod(int a)
        {
            return 5 + a;
        }
    }
}
