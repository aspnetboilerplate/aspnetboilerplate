using System.Text;
using Abp.Dependency;
using Abp.Runtime.Session;

namespace Abp.Web.Sessions
{
    public class SessionScriptManager : ISessionScriptManager, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }

        public SessionScriptManager()
        {
            AbpSession = NullAbpSession.Instance;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();

            script.AppendLine("    abp.session = abp.session || {};");

            if (AbpSession.UserId.HasValue)
            {
                script.AppendLine("    abp.session.userId = " + AbpSession.UserId.Value + ";");
            }

            if (AbpSession.TenantId.HasValue)
            {
                script.AppendLine("    abp.session.tenantId = " + AbpSession.TenantId.Value + ";");
            }

            script.AppendLine("    abp.session.multiTenancySide = " + ((int)AbpSession.MultiTenancySide) + ";");

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}