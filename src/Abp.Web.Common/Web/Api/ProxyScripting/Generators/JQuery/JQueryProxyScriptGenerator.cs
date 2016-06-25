using System.Text;
using Abp.Dependency;
using Abp.Web.Api.Modeling;

namespace Abp.Web.Api.ProxyScripting.Generators.JQuery
{
    public class JQueryProxyScriptGenerator : IProxyScriptGenerator, ITransientDependency
    {
        /// <summary>
        /// "jquery"
        /// </summary>
        public const string Name = "jquery";

        public string CreateScript(ApplicationApiDescriptionModel model)
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();

            foreach (var module in model.Modules.Values)
            {
                AddModuleScript(script, module);
            }

            script.AppendLine();
            script.AppendLine("})();");

            return script.ToString();
        }

        private void AddModuleScript(StringBuilder script, ModuleApiDescriptionModel module)
        {
            script.AppendLine("(function(){");
            script.AppendLine();

            foreach (var controller in module.Controllers.Values)
            {
                AddControllerScript(script, module, controller);
            }

            script.AppendLine();
            script.AppendLine("})();");
        }

        private void AddControllerScript(StringBuilder script, ModuleApiDescriptionModel module, ControllerApiDescriptionModel controller)
        {
            foreach (var action in controller.Actions.Values)
            {
                AddActionScript(script, module, controller, action);
            }
        }

        private void AddActionScript(StringBuilder script, ModuleApiDescriptionModel module, ControllerApiDescriptionModel controller, ActionApiDescriptionModel action)
        {
            script.AppendLine(action.Name);
        }
    }
}