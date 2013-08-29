using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Localization;
using Castle.Core.Logging;

namespace Abp.Web.Controllers
{
    public abstract class AbpApiController : ApiController
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
