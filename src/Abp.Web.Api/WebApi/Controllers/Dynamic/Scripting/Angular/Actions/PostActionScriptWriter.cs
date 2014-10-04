using System.Text;
using Abp.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.Angular.Actions
{
    internal class PostActionScriptWriter : ActionScriptWriter
    {
        public PostActionScriptWriter(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
            : base(controllerInfo, methodInfo)
        {

        }

        public override void WriteTo(StringBuilder script)
        {
            script.AppendLine("                        method: 'POST',");
            script.AppendLine("                        data: JSON.stringify(" + GeneratePostData() + ")");
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
                sb.AppendLine("                            " + parameterInfo.Name.ToCamelCase() + ": " + parameterInfo.Name.ToCamelCase() + (i < parameters.Length - 1 ? "," : ""));
            }
            sb.Append("                        }");

            return sb.ToString();
        }
    }
}