using System.Text;
using Abp.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.Angular.Actions
{
    internal class DeleteActionScriptWriter : ActionScriptWriter
    {
        public DeleteActionScriptWriter(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
            : base(controllerInfo, methodInfo)
        {

        }

        public override void WriteTo(StringBuilder script)
        {
            script.AppendLine("                        method: 'DELETE'");
        }

        public override string GetUrl()
        {
            var baseUrl = base.GetUrl();

            var urlBuilder = new StringBuilder(baseUrl);
            if (!baseUrl.Contains("?"))
            {
                urlBuilder.Append("?");
            }

            var parameters = MethodInfo.Method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterInfo = parameters[i];
                if (i > 0)
                {
                    urlBuilder.Append("&");
                }

                urlBuilder.Append(parameterInfo.Name.ToCamelCase() + "=' + escape(" + parameterInfo.Name.ToCamelCase() + ") + '");
            }
            return urlBuilder.ToString();
        }
    }
}