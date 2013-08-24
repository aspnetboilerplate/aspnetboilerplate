using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Data;
using Abp.Entities;
using Abp.Entities.Core;
using Abp.Services;
using Abp.Services.Core;
using AttributeRouting.Web.Http;

namespace Abp.Web.Controllers
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
        public virtual IEnumerable<User> Get()
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
