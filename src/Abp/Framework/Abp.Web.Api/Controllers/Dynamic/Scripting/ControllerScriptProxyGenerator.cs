using System.Text;
using Abp.Utils.Extensions;
using Abp.WebApi.Controllers.Dynamic.Scripting.Actions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    internal class ControllerScriptProxyGenerator
    {
        public string GenerateFor(DynamicApiControllerInfo controllerInfo, bool defineAmdModule = true)
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();
            script.AppendLine("    var serviceNamespace = abp.utils.createNamespace(abp, 'services." + controllerInfo.ServiceName.Replace("/", ".") + "');");
            script.AppendLine();

            //generate all methods
            foreach (var methodInfo in controllerInfo.Actions.Values)
            {
                AppendMethod(script, controllerInfo, methodInfo);
                script.AppendLine();
            }

            //generate amd module definition
            if (defineAmdModule)
            {
                script.AppendLine("    if(define && typeof define === 'function' && define.amd){");
                script.AppendLine("        define(function (require, exports, module) {");
                script.AppendLine("            return {");

                var methodNo = 0;
                foreach (var methodInfo in controllerInfo.Actions.Values)
                {
                    script.AppendLine("                " + methodInfo.ActionName.ToCamelCase() + ": serviceNamespace." + methodInfo.ActionName.ToCamelCase() + ((methodNo++) < (controllerInfo.Actions.Count - 1) ? "," : ""));
                }

                script.AppendLine("            };");
                script.AppendLine("        });");
                script.AppendLine("    }");
            }

            script.AppendLine();
            script.AppendLine("})();");

            return script.ToString();
        }

        private static void AppendMethod(StringBuilder script, DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
        {
            var generator = methodInfo.Verb.CreateActionScriptProxyGenerator(controllerInfo, methodInfo);
            script.AppendLine(generator.GenerateMethod());
        }
    }
}