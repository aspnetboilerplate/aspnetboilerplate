using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Abp.WebApi.Controllers.Dynamic;
using Abp.Utils.Extensions;

namespace Abp.Web.Mvc.Controllers.Dyanmic
{
    /// <summary>
    /// AbpWebApiJsGenerate/GenerateApiControllerProxy?serviceName=task
    /// </summary>
    public class AbpWebApiJsGenerateController : AbpController
    {
        public ActionResult GenerateApiControllerProxy(string serviceName)
        {
            var controllerInfo = DynamicApiControllerManager.FindServiceController(serviceName.ToPascalCase());
            if (controllerInfo == null)
            {
                throw new HttpException(404, "There is no such a service: " + serviceName); //TODO: What to do if can not find?
            }

            var script = new DynamicScriptGenerator().GenerateFor(controllerInfo);
            return JavaScript(script);
        }
    }
}
