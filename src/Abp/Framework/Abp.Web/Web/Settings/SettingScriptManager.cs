using System.Linq;
using System.Text;
using Abp.Configuration;
using Abp.Dependency;

namespace Abp.Web.Settings
{
    /// <summary>
    /// This class is used to build setting script.
    /// </summary>
    public class SettingScriptManager : ISettingScriptManager, ISingletonDependency
    {
        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly ISettingManager _settingManager;

        public SettingScriptManager(ISettingDefinitionManager settingDefinitionManager, ISettingManager settingManager)
        {
            _settingDefinitionManager = settingDefinitionManager;
            _settingManager = settingManager;
        }

        public string GetSettingScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");

            script.AppendLine("    abp.setting.values = {");

            var settingDefinitions = _settingDefinitionManager
                .GetAllSettingDefinitions()
                .Where(sd => sd.IsVisibleToClients);

            var added = 0;
            foreach (var settingDefinition in settingDefinitions)
            {
                if (added > 0)
                {
                    script.AppendLine(",");
                }
                else
                {
                    script.AppendLine();
                }

                script.Append("        '" + settingDefinition.Name.Replace("'", @"\'") + "': '" + _settingManager.GetSettingValue(settingDefinition.Name).Replace("'", @"\'") + "'");

                ++added;
            }

            script.AppendLine();
            script.AppendLine("    };");

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}