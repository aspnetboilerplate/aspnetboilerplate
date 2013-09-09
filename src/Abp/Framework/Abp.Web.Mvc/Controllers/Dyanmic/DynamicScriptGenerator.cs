using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Abp.WebApi.Controllers.Dynamic;
using Abp.Utils.Extensions;

namespace Abp.Web.Mvc.Controllers.Dyanmic
{
    internal class DynamicScriptGenerator
    {
        public string GenerateFor(DynamicApiControllerInfo controllerInfo)
        {
            var script = new StringBuilder();

            //Module dependencies and start
            script.AppendLine("define(['jquery', 'abp/abp'], function ($, abp) {");
            script.AppendLine();

            //all methods
            foreach (var methodInfo in controllerInfo.Methods.Values)
            {
                AppendMethod(script, controllerInfo, methodInfo);
                script.AppendLine();
            }

            //Return value of the module
            script.AppendLine("    return {");

            var methodNo = 0;
            foreach (var methodInfo in controllerInfo.Methods.Values)
            {
                script.AppendLine("        " + methodInfo.Name.ToCamelCase() + ": " + methodInfo.Name.ToCamelCase() + ((methodNo++) < (controllerInfo.Methods.Count - 1) ? "," : ""));
            }

            script.AppendLine("    };");
            script.AppendLine();

            //Module end
            script.AppendLine("});");

            return script.ToString();
        }

        private void AppendMethod(StringBuilder script, DynamicApiControllerInfo controllerInfo, DynamicApiMethodInfo methodInfo)
        {
            script.AppendLine("    var " + methodInfo.Name.ToCamelCase() + " = function(" + GenerateMethodParams(methodInfo.Method) + ") {");
            script.AppendLine("        return abp.ajax({");
            script.AppendLine("            url: '/api/services/" + controllerInfo.Name.ToCamelCase() + "/" + methodInfo.Name.ToCamelCase() + "',");
            script.Append("            type: '" + methodInfo.Verb.ToString().ToUpper() + "'");

            var parameters = methodInfo.Method.GetParameters();
            if (parameters.Length > 0)
            {
                script.AppendLine(",");
                //if (methodInfo.Verb == HttpVerb.Get || methodInfo.Verb == HttpVerb.Delete)
                //{
                //    script.AppendLine("            data: {");

                //    for (var i = 0; i < parameters.Length; i++)
                //    {
                //        var parameterInfo = parameters[i];
                //        script.AppendLine("                " + parameterInfo.Name.ToCamelCase() + ": " + parameterInfo.Name.ToCamelCase() + (i < parameters.Length - 1 ? "," : ""));
                //    }

                //    script.AppendLine("            }");
                //}
                //else if (methodInfo.Verb == HttpVerb.Post || methodInfo.Verb == HttpVerb.Put)
                //{
                    script.AppendLine("            data: JSON.stringify(" + parameters[0].Name.ToCamelCase() + ")");
                //}
            }
            else
            {
                script.AppendLine();
            }

            script.AppendLine("        });");
            script.AppendLine("    };");
        }

        private string GenerateMethodParams(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters.Length <= 0)
            {
                return "";
            }

            return string.Join(", ", parameters.Select(prm => prm.Name.ToCamelCase()));
        }
    }
}