using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.TypeScript
{
    public class TypeScriptDefinitionGenerator : ITransientDependency
    {
        public string GetScript()
        {
            var dynamicControllers = DynamicApiControllerManager.GetAll();
            StringBuilder script = new StringBuilder();
            if (dynamicControllers == null || dynamicControllers.Count == 0)
                return "";
            //TODO:Change the 'appServices' to the service prefix
            script.AppendLine("declare module abp.services.appServices");
            script.AppendLine("{");
            var proxyGenerator = new TypeScriptProxyGenerator();
            foreach (var dynamicController in dynamicControllers)
            {

                script.AppendLine(proxyGenerator.Generate(dynamicController));
                script.AppendLine();
            }
            script.AppendLine("}");
            return script.ToString();
        }
    }
}
