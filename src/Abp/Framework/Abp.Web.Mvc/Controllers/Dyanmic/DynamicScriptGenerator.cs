using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Abp.Web.Mvc.Controllers.Dyanmic
{
    public class DynamicScriptGenerator
    {
        public DynamicScriptGenerator()
        {

        }

        public string GenerateFor(Type type)
        {
            var script = new StringBuilder();
            script.AppendLine("define(['jquery', 'knockout', 'abp/abp'], function ($, ko, abp) {");

            foreach (var methodInfo in type.GetMethods())
            {
                AppendMethod(script, methodInfo);
                script.AppendLine();
            }

            script.AppendLine("});");

            return script.ToString();
        }

        private void AppendMethod(StringBuilder script, MethodInfo methodInfo)
        {
            script.AppendLine("    var " + methodInfo.Name + " = function(" + GenerateMethodParams(methodInfo) + ")");
            script.AppendLine("        return abp.ajax({");
            script.AppendLine("            url: '/api/services/Task/" + methodInfo.Name + "',");
            script.AppendLine("            type: 'GET'");
            script.AppendLine("        });");
            script.AppendLine("    }");
        }

        private string GenerateMethodParams(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters.Length <= 0)
            {
                return "";
            }

            return string.Join(", ", parameters.Select(prm => prm.Name));
        }
    }
}