using Abp.Modules.Core.Application.Services;
using Abp.Modules.Core.Mvc.Controllers;

namespace Taskever.Web.Controllers
{
    public class AccountController : AbpAccountController
    {
        public AccountController(IUserService userService)
            : base(userService)
        {

        }
    }
}
