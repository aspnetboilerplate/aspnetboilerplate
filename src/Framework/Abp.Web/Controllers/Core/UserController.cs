using System.Collections.Generic;
using Abp.Data;
using Abp.Entities.Core;
using Abp.Services.Core;
using Abp.Services.Core.Dto;
using AttributeRouting.Web.Http;

namespace Abp.Web.Controllers.Core
{
    public class UserApiController : AbpApiController
    {
        private readonly IUserService _userService;

        public UserApiController(IUserService userService)
        {
            _userService = userService;
        }

        [GET("Users/List")]
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
