using System.Linq;
using System.Reflection;
using System.Text;
using Abp.Utils.Extensions;
using Abp.Web;

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
            script.AppendLine("    abpModule.factory('services.tasksystem.task', [");
            script.AppendLine("        '$http', function ($http) {");
            script.AppendLine("            return new function () {");
            foreach (var methodInfo in _controllerInfo.Actions.Values)
            {
                var writer = CreateActionScriptWriter(_controllerInfo, methodInfo);
                writer.WriteTo(script);
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

        private static ActionScriptWriter CreateActionScriptWriter(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
        {
            switch (methodInfo.Verb)
            {
                case HttpVerb.Get:
                    return new PostActionScriptWriter(controllerInfo, methodInfo);
                case HttpVerb.Post:
                    return new PostActionScriptWriter(controllerInfo, methodInfo);
                case HttpVerb.Put:
                    return new PostActionScriptWriter(controllerInfo, methodInfo);
                case HttpVerb.Delete:
                    return new PostActionScriptWriter(controllerInfo, methodInfo);
                default:
                    throw new AbpException("This Http verb is not implemented: " + methodInfo.Verb);
            }
        }
    }

    internal abstract class ActionScriptWriter
    {
        protected readonly DynamicApiControllerInfo ControllerInfo;
        protected readonly DynamicApiActionInfo MethodInfo;

        protected ActionScriptWriter(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
        {
            ControllerInfo = controllerInfo;
            MethodInfo = methodInfo;
        }

        public virtual void WriteTo(StringBuilder script)
        {

        }
    }

    internal class PostActionScriptWriter : ActionScriptWriter
    {
        public PostActionScriptWriter(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
            : base(controllerInfo, methodInfo)
        {

        }

        public override void WriteTo(StringBuilder script)
        {
            script.AppendLine("                this." + MethodInfo.ActionName.ToCamelCase() + " = function (" + GenerateJsMethodParameterList(MethodInfo.Method) + ") {");
            script.AppendLine("                    return $http({");
            script.AppendLine("                        url: '" + PlainActionUrl + "',");
            script.AppendLine("                        method: 'POST',");
            script.AppendLine("                        data: JSON.stringify(" + GeneratePostData() + "),");
            script.AppendLine("                        abp: true");
            script.AppendLine("                    };");
            script.AppendLine("                };");
        }
        private string GeneratePostData()
        {
            var parameters = MethodInfo.Method.GetParameters();
            if (parameters.Length <= 0)
            {
                return "{}";
            }

            if (parameters.Length == 1)
            {
                return parameters[0].Name.ToCamelCase();
            }

            var sb = new StringBuilder();
            sb.AppendLine("{");
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterInfo = parameters[i];
                sb.AppendLine("                " + parameterInfo.Name.ToCamelCase() + ": " + parameterInfo.Name.ToCamelCase() + (i < parameters.Length - 1 ? "," : ""));
            }
            sb.AppendLine("}");

            return sb.ToString();
        }

        protected string GenerateJsMethodParameterList(MethodInfo methodInfo)
        {
            var paramNames = methodInfo.GetParameters().Select(prm => prm.Name.ToCamelCase()).ToList();
            paramNames.Add("httpParams");
            return string.Join(", ", paramNames);
        }

        protected virtual string PlainActionUrl
        {
            get
            {
                return "/api/services/" + ControllerInfo.ServiceName + "/" + MethodInfo.ActionName;
            }
        }
    }
}