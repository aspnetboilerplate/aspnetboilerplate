using System.Text;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Json;

namespace Abp.Web.Configuration
{
    public class CustomConfigScriptManager : ICustomConfigScriptManager, ITransientDependency
    {
        private readonly IAbpStartupConfiguration _abpStartupConfiguration;

        public CustomConfigScriptManager(IAbpStartupConfiguration abpStartupConfiguration)
        {
            _abpStartupConfiguration = abpStartupConfiguration;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(abp){");
            script.AppendLine();

            script.AppendLine("    abp.custom = " + _abpStartupConfiguration.GetCustomConfig().ToJsonString());

            script.AppendLine();
            script.Append("})(abp);");

            return script.ToString();
        }
    }
}