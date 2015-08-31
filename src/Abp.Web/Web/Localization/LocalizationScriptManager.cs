using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using Abp.Dependency;
using Abp.Localization;
using Abp.Runtime.Caching.Memory;

namespace Abp.Web.Localization
{
    internal class LocalizationScriptManager : ILocalizationScriptManager, ISingletonDependency
    {
        private readonly ILocalizationManager _localizationManager;

        private readonly ThreadSafeObjectCache<string> _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="Abp.Web.Localization.LocalizationScriptManager"/> class.
        /// </summary>
        /// <param name="localizationManager">Localization manager.</param>
        public LocalizationScriptManager(ILocalizationManager localizationManager)
        {
            _localizationManager = localizationManager;
            _cache = new ThreadSafeObjectCache<string>(new MemoryCache("__LocalizationScriptManager"), TimeSpan.FromDays(1));
        }

        /// <inheritdoc/>
        public string GetScript()
        {
            return GetScript(Thread.CurrentThread.CurrentUICulture);
        }

        /// <inheritdoc/>
        public string GetScript(CultureInfo cultureInfo)
        {
            return _cache.Get(cultureInfo.Name, BuildAll);
        }

        private string BuildAll()
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
            script.Append("    abp.localization.languages = [");

            var languages = _localizationManager.GetAllLanguages();
            for (var i = 0; i < languages.Count; i++)
            {
                var language = languages[i];

                script.AppendLine("{");
                script.AppendLine("        name: '" + language.Name + "',");
                script.AppendLine("        displayName: '" + language.DisplayName + "',");
                script.AppendLine("        icon: '" + language.Icon + "',");
                script.AppendLine("        isDefault: " + language.IsDefault.ToString().ToLower());
                script.Append("    }");

                if (i < languages.Count - 1)
                {
                    script.Append(" , ");
                }
            }

            script.AppendLine("];");
            script.AppendLine();

            if (languages.Count > 0)
            {
                var currentLanguage = _localizationManager.CurrentLanguage;
                script.AppendLine("    abp.localization.currentLanguage = {");
                script.AppendLine("        name: '" + currentLanguage.Name + "',");
                script.AppendLine("        displayName: '" + currentLanguage.DisplayName + "',");
                script.AppendLine("        icon: '" + currentLanguage.Icon + "',");
                script.AppendLine("        isDefault: " + currentLanguage.IsDefault.ToString().ToLower());
                script.AppendLine("    };");
            }

            script.AppendLine();
            script.AppendLine("    abp.localization.values = abp.localization.values || {};");
            script.AppendLine();

            foreach (var source in _localizationManager.GetAllSources().OrderBy(s => s.Name))
            {
                script.AppendLine("    abp.localization.values['" + source.Name + "'] = {");

                var stringValues = source.GetAllStrings().OrderBy(s => s.Name).ToList();
                for (var i = 0; i < stringValues.Count; i++)
                {
                    script.AppendLine(
                        string.Format(
                            "        '{0}' : '{1}'" + (i < stringValues.Count - 1 ? "," : ""),
                                stringValues[i].Name,
                                stringValues[i].Value
                                    .Replace(@"\", @"\\")
                                    .Replace("'", @"\'")
                                    .Replace(Environment.NewLine, string.Empty)
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
