using System.Globalization;
using System.Text;
using Abp.Extensions;
using Abp.Web;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.Angular
{
    internal class AngularActionScriptWriter
    {
        private readonly DynamicApiControllerInfo _controllerInfo;
        private readonly DynamicApiActionInfo _actionInfo;

        public AngularActionScriptWriter(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
        {
            _controllerInfo = controllerInfo;
            _actionInfo = methodInfo;
        }

        public virtual void WriteTo(StringBuilder script)
        {
            script.AppendLine("                this." + _actionInfo.ActionName.ToCamelCase() + " = function (" + ActionScriptingHelper.GenerateJsMethodParameterList(_actionInfo.Method, "httpParams") + ") {");
            script.AppendLine("                    return $http(angular.extend({");
            script.AppendLine("                        url: abp.appPath + '" + ActionScriptingHelper.GenerateUrlWithParameters(_controllerInfo, _actionInfo) + "',");
            script.AppendLine("                        method: '" + _actionInfo.Verb.ToString().ToUpper(CultureInfo.InvariantCulture) + "',");

            if (_actionInfo.Verb == HttpVerb.Get)
            {
                script.AppendLine("                        params: " + ActionScriptingHelper.GenerateBody(_actionInfo));
            }
            else
            {
                script.AppendLine("                        data: JSON.stringify(" + ActionScriptingHelper.GenerateBody(_actionInfo) + ")");
            }

            script.AppendLine("                    }, httpParams));");
            script.AppendLine("                };");
            script.AppendLine("                ");
        }
    }
}