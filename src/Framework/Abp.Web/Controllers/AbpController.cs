using System.Web.Mvc;
using Abp.Localization;
using Castle.Core.Logging;

namespace Abp.Web.Controllers
{
    public abstract class AbpController : Controller
    {
        public ILogger Logger { get; set; }

        public ILocalizationManager LocalizationManager
        {
            get { return _localizationManager; }
            set { _localizationManager = value; }
        }
        private ILocalizationManager _localizationManager = NullLocalizationManager.Instance;

        public string L(string name)
        {
            return LocalizationManager.GetString(name);
        }

        public string L(string name, string languageCode)
        {
            return LocalizationManager.GetString(name, languageCode);
        }
    }
}