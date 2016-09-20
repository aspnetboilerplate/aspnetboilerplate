using System.Text;
using Abp.Dependency;
using Abp.Web.Security.AntiForgery;

namespace Abp.Web.Security
{
    internal class SecurityScriptManager : ISecurityScriptManager, ITransientDependency
    {
        private readonly IAbpAntiForgeryConfiguration _abpAntiForgeryConfiguration;

        public SecurityScriptManager(IAbpAntiForgeryConfiguration abpAntiForgeryConfiguration)
        {
            _abpAntiForgeryConfiguration = abpAntiForgeryConfiguration;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine("    abp.security.antiForgery.tokenCookieName = '" + _abpAntiForgeryConfiguration.TokenCookieName + "';");
            script.AppendLine("    abp.security.antiForgery.tokenHeaderName = '" + _abpAntiForgeryConfiguration.TokenHeaderName + "';");
            script.Append("})();");

            return script.ToString();
        }
    }
}
