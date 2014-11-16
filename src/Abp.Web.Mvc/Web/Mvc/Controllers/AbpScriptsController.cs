using System.Text;
using System.Web.Mvc;
using Abp.Web.Authorization;
using Abp.Web.Localization;
using Abp.Web.Navigation;
using Abp.Web.Settings;

namespace Abp.Web.Mvc.Controllers
{
    public class AbpScriptsController : AbpController
    {
        private readonly ISettingScriptManager _settingScriptManager;
        private readonly INavigationScriptManager _navigationScriptManager;
        private readonly ILocalizationScriptManager _localizationScriptManager;
        private readonly IAuthorizationScriptManager _authorizationScriptManager;

        public AbpScriptsController(ISettingScriptManager settingScriptManager, INavigationScriptManager navigationScriptManager, ILocalizationScriptManager localizationScriptManager, IAuthorizationScriptManager authorizationScriptManager)
        {
            _settingScriptManager = settingScriptManager;
            _navigationScriptManager = navigationScriptManager;
            _localizationScriptManager = localizationScriptManager;
            _authorizationScriptManager = authorizationScriptManager;
        }

        public ActionResult GetScripts()
        {
            var sb = new StringBuilder();

            sb.AppendLine(_localizationScriptManager.GetLocalizationScript());
            sb.AppendLine();
            sb.AppendLine(_authorizationScriptManager.GetAuthorizationScript());
            sb.AppendLine();
            sb.AppendLine(_navigationScriptManager.GetScript());
            sb.AppendLine();
            sb.AppendLine(_settingScriptManager.GetSettingScript());

            return Content(sb.ToString(), "application/x-javascript", Encoding.UTF8);
        }
    }
}
