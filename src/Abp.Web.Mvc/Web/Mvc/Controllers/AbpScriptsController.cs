using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Auditing;
using Abp.Extensions;
using Abp.Web.Authorization;
using Abp.Web.Features;
using Abp.Web.Localization;
using Abp.Web.MultiTenancy;
using Abp.Web.Navigation;
using Abp.Web.Security;
using Abp.Web.Sessions;
using Abp.Web.Settings;
using Abp.Web.Timing;

namespace Abp.Web.Mvc.Controllers
{
    /// <summary>
    /// This controller is used to create client side scripts
    /// to work with ABP.
    /// </summary>
    public class AbpScriptsController : AbpController
    {
        private readonly IMultiTenancyScriptManager _multiTenancyScriptManager;
        private readonly ISettingScriptManager _settingScriptManager;
        private readonly INavigationScriptManager _navigationScriptManager;
        private readonly ILocalizationScriptManager _localizationScriptManager;
        private readonly IAuthorizationScriptManager _authorizationScriptManager;
        private readonly IFeaturesScriptManager _featuresScriptManager;
        private readonly ISessionScriptManager _sessionScriptManager;
        private readonly ITimingScriptManager _timingScriptManager;
        private readonly ISecurityScriptManager _securityScriptManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpScriptsController(
            IMultiTenancyScriptManager multiTenancyScriptManager,
            ISettingScriptManager settingScriptManager,
            INavigationScriptManager navigationScriptManager,
            ILocalizationScriptManager localizationScriptManager,
            IAuthorizationScriptManager authorizationScriptManager,
            IFeaturesScriptManager featuresScriptManager,
            ISessionScriptManager sessionScriptManager, 
            ITimingScriptManager timingScriptManager,
            ISecurityScriptManager securityScriptManager)
        {
            _multiTenancyScriptManager = multiTenancyScriptManager;
            _settingScriptManager = settingScriptManager;
            _navigationScriptManager = navigationScriptManager;
            _localizationScriptManager = localizationScriptManager;
            _authorizationScriptManager = authorizationScriptManager;
            _featuresScriptManager = featuresScriptManager;
            _sessionScriptManager = sessionScriptManager;
            _timingScriptManager = timingScriptManager;
            _securityScriptManager = securityScriptManager;
        }

        /// <summary>
        /// Gets all needed scripts.
        /// </summary>
        [DisableAuditing]
        public async Task<ActionResult> GetScripts(string culture = "")
        {
            if (!culture.IsNullOrEmpty())
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            }

            //TODO: Optimize this using single StringBuilde

            var sb = new StringBuilder();

            sb.AppendLine(_multiTenancyScriptManager.GetScript());
            sb.AppendLine();

            sb.AppendLine(_sessionScriptManager.GetScript());
            sb.AppendLine();

            sb.AppendLine(_localizationScriptManager.GetScript());
            sb.AppendLine();

            sb.AppendLine(await _featuresScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(await _authorizationScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(await _navigationScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(await _settingScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(await _timingScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(_securityScriptManager.GetScript());
            sb.AppendLine();

            sb.AppendLine(GetTriggerScript());

            return Content(sb.ToString(), "application/x-javascript", Encoding.UTF8);
        }

        private static string GetTriggerScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine("    abp.event.trigger('abp.dynamicScriptsInitialized');");
            script.Append("})();");

            return script.ToString();
        }
    }
}
