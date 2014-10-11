using System.Text;
using System.Web.Mvc;
using Abp.Web.Navigation;

namespace Abp.Web.Mvc.Controllers.Navigation
{
    public class AbpNavigationController : AbpController
    {
        private readonly INavigationScriptManager navigationScriptManager;

        public AbpNavigationController(INavigationScriptManager navigationScriptManager)
        {
            this.navigationScriptManager = navigationScriptManager;
        }

        public ContentResult GetScripts()
        {
            var script = navigationScriptManager.GetScript();
            return Content(script, "application/x-javascript", Encoding.UTF8);
        }
    }
}
