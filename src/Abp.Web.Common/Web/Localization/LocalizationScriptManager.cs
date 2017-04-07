using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Abp.Dependency;
using Abp.Json;
using Abp.Localization;
using Abp.Runtime.Caching;

namespace Abp.Web.Localization
{
    internal class LocalizationScriptManager : ILocalizationScriptManager, ISingletonDependency
    {
        private readonly ILocalizationManager _localizationManager;
        private readonly ICacheManager _cacheManager;
        private readonly ILanguageManager _languageManager;

        public LocalizationScriptManager(
            ILocalizationManager localizationManager, 
            ICacheManager cacheManager,
            ILanguageManager languageManager)
        {
            _localizationManager = localizationManager;
            _cacheManager = cacheManager;
            _languageManager = languageManager;
        }

        /// <inheritdoc/>
        public string GetScript()
        {
            return GetScript(Thread.CurrentThread.CurrentUICulture);
        }

        /// <inheritdoc/>
        public string GetScript(CultureInfo cultureInfo)
        {
            //NOTE: Disabled caching since it's not true (localization script is changed per user, per tenant, per culture...)
            return BuildAll(cultureInfo);
            //return _cacheManager.GetCache(AbpCacheNames.LocalizationScripts).Get(cultureInfo.Name, () => BuildAll(cultureInfo));
        }

        private string BuildAll(CultureInfo cultureInfo)
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();
            script.AppendLine("    abp.localization = abp.localization || {};");
            script.AppendLine();
            script.AppendLine("    abp.localization.currentCulture = {");
            script.AppendLine("        name: '" + cultureInfo.Name + "',");
            script.AppendLine("        displayName: '" + cultureInfo.DisplayName + "'");
            script.AppendLine("    };");
            script.AppendLine();
            script.Append("    abp.localization.languages = [");

            var languages = _languageManager.GetLanguages();
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
                var currentLanguage = _languageManager.CurrentLanguage;
                script.AppendLine("    abp.localization.currentLanguage = {");
                script.AppendLine("        name: '" + currentLanguage.Name + "',");
                script.AppendLine("        displayName: '" + currentLanguage.DisplayName + "',");
                script.AppendLine("        icon: '" + currentLanguage.Icon + "',");
                script.AppendLine("        isDefault: " + currentLanguage.IsDefault.ToString().ToLower());
                script.AppendLine("    };");
            }

            var sources = _localizationManager.GetAllSources().OrderBy(s => s.Name).ToArray();

            script.AppendLine();
            script.AppendLine("    abp.localization.sources = [");

            for (int i = 0; i < sources.Length; i++)
            {
                var source = sources[i];
                script.AppendLine("        {");
                script.AppendLine("            name: '" + source.Name + "',");
                script.AppendLine("            type: '" + source.GetType().Name + "'");
                script.AppendLine("        }" + (i < (sources.Length - 1) ? "," : ""));
            }

            script.AppendLine("    ];");

            script.AppendLine();
            script.AppendLine("    abp.localization.values = abp.localization.values || {};");
            script.AppendLine();

            foreach (var source in sources)
            {
                script.Append("    abp.localization.values['" + source.Name + "'] = ");

                var stringValues = source.GetAllStrings(cultureInfo).OrderBy(s => s.Name).ToList();
                var stringJson = stringValues
                    .ToDictionary(_ => _.Name, _ => _.Value)
                    .ToJsonString(indented: true);
                script.Append(stringJson);

                script.AppendLine(";");
                script.AppendLine();
            }

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}
