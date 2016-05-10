using System.Text;
using Abp.Dependency;
using Abp.Runtime.Session;

namespace Abp.Web.Sessions
{
    public class SessionScriptManager : ISessionScriptManager, ITransientDependency
    {
        public SessionScriptManager()
        {
            AbpSession = NullAbpSession.Instance;
        }

        public IAbpSession AbpSession { get; set; }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();

            script.AppendLine("    abp.session = abp.session || {};");
            script.AppendLine("    abp.session.userId = " +
                              (AbpSession.UserId.HasValue ? "'" + AbpSession.UserId.Value + "'" : "null") + ";");
            script.AppendLine("    abp.session.tenantId = " +
                              (AbpSession.TenantId.HasValue ? "'" + AbpSession.TenantId.Value + "'" : "null") + ";");
            script.AppendLine("    abp.session.impersonatorUserId = " +
                              (AbpSession.ImpersonatorUserId.HasValue
                                  ? "'" + AbpSession.ImpersonatorUserId.Value + "'"
                                  : "null") + ";");
            script.AppendLine("    abp.session.impersonatorTenantId = " +
                              (AbpSession.ImpersonatorTenantId.HasValue
                                  ? "'" + AbpSession.ImpersonatorTenantId.Value + "'"
                                  : "null") + ";");
            script.AppendLine("    abp.session.multiTenancySide = " + (int) AbpSession.MultiTenancySide + ";");

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}