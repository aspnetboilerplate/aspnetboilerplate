using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<string> GetScriptAsync()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine("    abp.setting = abp.setting || {};");
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

                var settingValue = await _settingManager.GetSettingValueAsync(settingDefinition.Name);

                script.Append("        '" +
                              settingDefinition.Name .Replace("'", @"\'") + "': " +
                              (settingValue == null ? "null" : "'" + settingValue.Replace(@"\", @"\\").Replace("'", @"\'") + "'"));

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