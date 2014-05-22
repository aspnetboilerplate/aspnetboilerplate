using System.Text;
using System.Web.Mvc;
using Abp.Web.Settings;

namespace Abp.Web.Mvc.Controllers.Settings
{
    public class AbpSettingsController : Controller
    {
        private readonly ISettingScriptManager _settingScriptManager;

        public AbpSettingsController(ISettingScriptManager settingScriptManager)
        {
            _settingScriptManager = settingScriptManager;
        }

        public ActionResult GetScripts()
        {
            var script = _settingScriptManager.GetSettingScript();
            return Content(script, "application/x-javascript", Encoding.UTF8);
        }
    }
}