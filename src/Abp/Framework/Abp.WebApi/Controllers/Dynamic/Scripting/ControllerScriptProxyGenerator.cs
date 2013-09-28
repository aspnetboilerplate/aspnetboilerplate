using System.Linq;
using System.Reflection;
using System.Text;
using Abp.Utils.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    internal class ControllerScriptProxyGenerator
    {
        public string GenerateFor(DynamicApiControllerInfo controllerInfo)
        {
            var script = new StringBuilder();

            //Module dependencies and start
            script.AppendLine("define(['jquery'], function ($) {");

            //all methods
            foreach (var methodInfo in controllerInfo.Actions.Values)
            {
                script.AppendLine();
                AppendMethod(script, controllerInfo, methodInfo);
            }

            //Return value of the module
            script.AppendLine();
            script.AppendLine("    return {");

            var methodNo = 0;
            foreach (var methodInfo in controllerInfo.Actions.Values)
            {
                script.AppendLine("        " + methodInfo.ActionName.ToCamelCase() + ": " + methodInfo.ActionName.ToCamelCase() + ((methodNo++) < (controllerInfo.Actions.Count - 1) ? "," : ""));
            }

            script.AppendLine("    };");
            script.AppendLine();

            //Module end
            script.AppendLine("});");

            return script.ToString();
        }

        private void AppendMethod(StringBuilder script, DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
        {
            var generator = methodInfo.Verb.CreateActionScriptProxyGenerator(controllerInfo, methodInfo);
            script.AppendLine(generator.GenerateMethod());
        }
    }
}