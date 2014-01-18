using System.Text;
using Abp.Utils.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.Actions
{
    internal class HttpGetActionScriptProxyGenerator : ActionScriptProxyGenerator
    {
        private const string AjaxPatametersTemplate = 
@"            url: '{url}',
            type: '{type}',
            data: {
{getParams}
            }";

        public HttpGetActionScriptProxyGenerator(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
            : base(controllerInfo, methodInfo)
        {
        }

        protected override string GenerateAjaxCallParameters()
        {
            var ajaxPatameters = AjaxPatametersTemplate
                .Replace("{url}", PlainActionUrl)
                .Replace("{type}", ActionInfo.Verb.ToString().ToUpperInvariant())
                .Replace("{getParams}", GenerateGetParameters());

            return ajaxPatameters;
        }

        private string GenerateGetParameters()
        {
            var sb = new StringBuilder();
            var parameters = ActionInfo.Method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterInfo = parameters[i];
                sb.AppendLine("                " + parameterInfo.Name.ToCamelCase() + ": " + parameterInfo.Name.ToCamelCase() + (i < parameters.Length - 1 ? "," : ""));
            }
            return sb.ToString();
        }
    }
}