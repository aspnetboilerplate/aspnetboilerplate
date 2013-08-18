using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Localization;

namespace Abp.Web.Controllers
{
    public class AbpApiController : ApiController
    {
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
