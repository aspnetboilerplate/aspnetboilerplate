using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Web.Authorization;
using Abp.Web.Localization;
using Abp.Web.Navigation;
using Abp.Web.Sessions;
using Abp.Web.Settings;

namespace Abp.Web.Mvc.Controllers
{
    public class AbpScriptsController : AbpController
    {
        private readonly ISettingScriptManager _settingScriptManager;
        private readonly INavigationScriptManager _navigationScriptManager;
        private readonly ILocalizationScriptManager _localizationScriptManager;
        private readonly IAuthorizationScriptManager _authorizationScriptManager;
        private readonly ISessionScriptManager _sessionScriptManager;

        public AbpScriptsController(
            ISettingScriptManager settingScriptManager, 
            INavigationScriptManager navigationScriptManager, 
            ILocalizationScriptManager localizationScriptManager, 
            IAuthorizationScriptManager authorizationScriptManager, 
            ISessionScriptManager sessionScriptManager)
        {
            _settingScriptManager = settingScriptManager;
            _navigationScriptManager = navigationScriptManager;
            _localizationScriptManager = localizationScriptManager;
            _authorizationScriptManager = authorizationScriptManager;
            _sessionScriptManager = sessionScriptManager;
        }

        public async Task<ActionResult> GetScripts()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine(_sessionScriptManager.GetScript());
            sb.AppendLine();
            
            sb.AppendLine(_localizationScriptManager.GetScript());
            sb.AppendLine();
            
            sb.AppendLine(await _authorizationScriptManager.GetScriptAsync());
            sb.AppendLine();
            
            sb.AppendLine(_navigationScriptManager.GetScript());
            sb.AppendLine();
            
            sb.AppendLine(_settingScriptManager.GetScript());

            return Content(sb.ToString(), "application/x-javascript", Encoding.UTF8);
        }
    }
}
