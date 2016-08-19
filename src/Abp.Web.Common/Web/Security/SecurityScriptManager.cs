using System.Text;
using Abp.Dependency;

namespace Abp.Web.Security
{
    internal class SecurityScriptManager : ISecurityScriptManager, ITransientDependency
    {
        private readonly ICsrfConfiguration _csrfConfiguration;

        public SecurityScriptManager(ICsrfConfiguration csrfConfiguration)
        {
            _csrfConfiguration = csrfConfiguration;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine("    abp.security.csrfTokenCookieName = '" + _csrfConfiguration.TokenCookieName + "';");
            script.AppendLine("    abp.security.csrfTokenHeaderName = '" + _csrfConfiguration.TokenHeaderName + "';");
            script.Append("})();");

            return script.ToString();
        }
    }
}
