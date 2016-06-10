using System;
using System.Globalization;
using System.Text;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Extensions;
using Abp.MultiTenancy;

namespace Abp.Web.MultiTenancy
{
    public class MultiTenancyScriptManager : IMultiTenancyScriptManager, ITransientDependency
    {
        private readonly IMultiTenancyConfig _multiTenancyConfig;

        public MultiTenancyScriptManager(IMultiTenancyConfig multiTenancyConfig)
        {
            _multiTenancyConfig = multiTenancyConfig;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(abp){");
            script.AppendLine();

            script.AppendLine("    abp.multiTenancy = abp.multiTenancy || {};");
            script.AppendLine("    abp.multiTenancy.isEnabled = " + _multiTenancyConfig.IsEnabled.ToString().ToLower(CultureInfo.InvariantCulture) + ";");
            
            var sideNames = Enum.GetNames(typeof (MultiTenancySides));

            script.AppendLine("    abp.multiTenancy.sides = {");
            for (int i = 0; i < sideNames.Length; i++)
            {
                var sideName = sideNames[i];
                script.Append("        " + sideName.ToCamelCase() + ": " + ((int) sideName.ToEnum<MultiTenancySides>()));
                if (i == sideNames.Length - 1)
                {
                    script.AppendLine();
                }
                else
                {
                    script.AppendLine(",");
                }
            }

            script.AppendLine("    };");

            script.AppendLine();
            script.Append("})(abp);");

            return script.ToString();
        }
    }
}