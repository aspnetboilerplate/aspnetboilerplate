using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using Abp.Dependency;
using Abp.Localization.Sources;
using Abp.Runtime.Caching;

namespace Abp.Web.Localization
{
    /// <summary>
    /// This class is used to build and cache localization script.
    /// </summary>
    public class LocalizationScriptManager : ILocalizationScriptManager, ISingletonDependency //TODO: Make it internal?
    {
        private readonly ILocalizationSourceManager _localizationSourceManager;

        private readonly ThreadSafeObjectCache<string> _cache;

        public LocalizationScriptManager(ILocalizationSourceManager localizationSourceManager)
        {
            _localizationSourceManager = localizationSourceManager;
            _cache = new ThreadSafeObjectCache<string>(new MemoryCache("__LocalizationScriptManager"), TimeSpan.FromDays(2));
        }

        public string GetLocalizationScript()
        {
            return GetLocalizationScript(Thread.CurrentThread.CurrentUICulture);
        }

        public string GetLocalizationScript(CultureInfo cultureInfo)
        {
            return _cache.Get(cultureInfo.Name, BuildAll);
        }

        public string BuildAll()
        {
            var script = new StringBuilder();
            var currentCulture = Thread.CurrentThread.CurrentUICulture;
            
            script.AppendLine("(function(){");
            script.AppendLine();
            script.AppendLine("    abp.localization = abp.localization || {};");
            script.AppendLine();
            script.AppendLine("    abp.localization.currentCulture = {");
            script.AppendLine("        name: '" + currentCulture.Name + "',");
            script.AppendLine("        displayName: '" + currentCulture.DisplayName + "'");
            script.AppendLine("    };");
            script.AppendLine();
            script.AppendLine("    abp.localization.values = abp.localization.values || {};");
            script.AppendLine();

            foreach (var source in _localizationSourceManager.GetAllSources().OrderBy(s => s.Name))
            {
                script.AppendLine("    abp.localization.values['" + source.Name + "'] = {");

                var stringValues = source.GetAllStrings().OrderBy(s => s.Name).ToList();
                for (var i = 0; i < stringValues.Count; i++)
                {
                    script.AppendLine(
                        string.Format(
                        "        '{0}' : '{1}'" + (i < stringValues.Count - 1 ? "," : ""),
                            stringValues[i].Name,
                            stringValues[i].Value.Replace("'", "\\'").Replace(Environment.NewLine, string.Empty) //TODO: Allow new line?
                            ));
                }

                script.AppendLine("    };");
                script.AppendLine();
            }

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}
