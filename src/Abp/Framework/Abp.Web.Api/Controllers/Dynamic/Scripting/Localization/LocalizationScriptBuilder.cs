using System;
using System.Collections.Generic;
using System.Text;
using Abp.Localization;
using Abp.Utils.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.Localization
{
    public class LocalizationScriptBuilder
    {
        private readonly List<ILocalizationSource> _sources;

        public LocalizationScriptBuilder(List<ILocalizationSource> sources)
        {
            _sources = sources;
        }

        public string BuildAll()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();
            script.AppendLine("abp.localization.values = abp.localization.values || {};");
            script.AppendLine();
            foreach (var source in _sources)
            {
                script.AppendLine("abp.localization.values['" + source.SourceName.ToCamelCase() + "'] = {");

                var stringValues = source.GetAllStrings();
                for (int i = 0; i < stringValues.Count; i++)
                {
                    script.AppendLine(
                        string.Format(
                        "    '{0}' : '{1}'" + (i < stringValues.Count - 1 ? "," : ""),
                            stringValues[i].Key,
                            stringValues[i].Value.Replace("'", "\\'").Replace(Environment.NewLine, string.Empty) //TODO: Allow new line?
                            ));
                }

                script.AppendLine("};");
                script.AppendLine();
            }

            script.AppendLine();
            script.AppendLine("})();");

            return script.ToString();
        }
    }
}