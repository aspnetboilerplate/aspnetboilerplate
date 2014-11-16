using System;
using System.Text;
using Abp.Dependency;
using Abp.Runtime.Session;

namespace Abp.Web.Sessions
{
    public class SessionScriptManager : ISessionScriptManager, ISingletonDependency
    {
        private readonly IAbpSession _abpSession;

        public SessionScriptManager(IAbpSession abpSession)
        {
            _abpSession = abpSession;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();

            script.AppendLine("    abp.session = abp.session || {};");
            
            if (_abpSession.UserId.HasValue)
            {
                script.AppendLine("    abp.session.userId = " + _abpSession.UserId.Value + ";");
            }

            if (_abpSession.TenantId.HasValue)
            {
                script.AppendLine("    abp.session.tenantId = " + _abpSession.TenantId.Value + ";");
            }

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}