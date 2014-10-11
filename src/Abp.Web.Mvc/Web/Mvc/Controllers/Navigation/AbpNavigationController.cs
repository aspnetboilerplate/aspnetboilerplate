using System.Text;
using System.Web.Mvc;
using Abp.Application.Navigation;

namespace Abp.Web.Mvc.Controllers.Navigation
{
    public class AbpNavigationController : AbpController
    {
        private readonly INavigationManager _navigationManager;

        public AbpNavigationController(INavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public ContentResult GetScripts()
        {
            var script = "";
            return Content(script, "application/x-javascript", Encoding.UTF8);
        }
    }
}
