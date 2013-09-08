using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Abp.WebApi.Controllers.Dynamic;

namespace Abp.Web.Mvc.Controllers.Dyanmic
{
    public class AbpWebApiJsGenerateController : AbpController
    {
        public ActionResult GenerateApiControllerProxy(string serviceName)
        {
            var controllerInfo = DynamicApiControllerManager.FindServiceController(serviceName);
            if (controllerInfo == null)
            {
                throw new HttpException(404, "There is no such a service: " + serviceName); //TODO: What to do if can not find?
            }

            var script = new DynamicScriptGenerator().GenerateFor(controllerInfo.ProxiedType);
            return JavaScript(script);
        }
    }
}
