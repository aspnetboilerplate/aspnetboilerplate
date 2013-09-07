using System.Collections.Generic;
using Abp.Data;
using Abp.Modules.Core.Services;
using Abp.Modules.Core.Services.Dto;
using Abp.Web.Controllers;
using AttributeRouting.Web.Http;

namespace Abp.Modules.Core.Controllers
{
    public class UserApiController : AbpApiController
    {
        private readonly IUserService _userService;

        public UserApiController(IUserService userService)
        {
            _userService = userService;
        }

        [GET("Users/List")] //NOTE: For test, remove it
        [UnitOfWork]
        public virtual IEnumerable<UserDto> Get()
        {
            //validation
            //throwing appropriate messages
            //logging
            //exception handling
            Logger.Info(L("GetAllQuestions_Method_Is_Called"));
            return _userService.GetAllUsers();
        }
    }
}
