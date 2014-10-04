using System.Linq;
using System.Reflection;
using System.Text;
using Abp.Extensions;
using Abp.Web;
using Abp.WebApi.Controllers.Dynamic.Scripting.Angular.Actions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.Angular
{
    internal class AngularProxyGenerator : IScriptProxyGenerator
    {
        private readonly DynamicApiControllerInfo _controllerInfo;

        public AngularProxyGenerator(DynamicApiControllerInfo controllerInfo)
        {
            _controllerInfo = controllerInfo;
        }

        public string Generate()
        {
            var script = new StringBuilder();

            script.AppendLine("(function (abp, angular) {");
            script.AppendLine("");
            script.AppendLine("    if (!angular) {");
            script.AppendLine("        return;");
            script.AppendLine("    }");
            script.AppendLine("    ");
            script.AppendLine("    var abpModule = angular.module('abp');");
            script.AppendLine("    ");
            script.AppendLine("    abpModule.factory('abp.services." + _controllerInfo.ServiceName.Replace("/", ".") + "', [");
            script.AppendLine("        '$http', function ($http) {");
            script.AppendLine("            return new function () {");

            foreach (var methodInfo in _controllerInfo.Actions.Values)
            {
                var actionWriter = CreateActionScriptWriter(_controllerInfo, methodInfo);

                script.AppendLine("                this." + methodInfo.ActionName.ToCamelCase() + " = function (" + GenerateJsMethodParameterList(methodInfo.Method) + ") {");
                script.AppendLine("                    return $http(angular.extend({");
                script.AppendLine("                        abp: true,");
                script.AppendLine("                        url: abp.appPath + '" + actionWriter.GetUrl() + "',");
                actionWriter.WriteTo(script);
                script.AppendLine("                    }, httpParams));");
                script.AppendLine("                };");
                script.AppendLine("                ");
            }

            script.AppendLine("            };");
            script.AppendLine("        }");
            script.AppendLine("    ]);");
            script.AppendLine();

            //generate all methods

            script.AppendLine();
            script.AppendLine("})((abp || (abp = {})), (angular || undefined));");

            return script.ToString();
        }

        protected string GenerateJsMethodParameterList(MethodInfo methodInfo)
        {
            var paramNames = methodInfo.GetParameters().Select(prm => prm.Name.ToCamelCase()).ToList();
            paramNames.Add("httpParams");
            return string.Join(", ", paramNames);
        }
        
        private static ActionScriptWriter CreateActionScriptWriter(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
        {
            switch (methodInfo.Verb)
            {
                case HttpVerb.Get:
                    return new GetActionScriptWriter(controllerInfo, methodInfo);
                case HttpVerb.Post:
                    return new PostActionScriptWriter(controllerInfo, methodInfo);
                case HttpVerb.Put:
                    return new PostActionScriptWriter(controllerInfo, methodInfo);
                case HttpVerb.Delete:
                    return new DeleteActionScriptWriter(controllerInfo, methodInfo);
                default:
                    throw new AbpException("This Http verb is not implemented: " + methodInfo.Verb);
            }
        }
    }
}